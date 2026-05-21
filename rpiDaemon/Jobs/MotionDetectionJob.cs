using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Shared;
using Microsoft.Extensions.Hosting;

namespace rpiDaemon.Jobs
{
    public class MotionDetectionJob : IJob
    {
        private readonly ILogger<MotionDetectionJob> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public MotionDetectionJob(ILogger<MotionDetectionJob> logger, IWebHostEnvironment env, IConfiguration config)
        {
            _logger = logger;
            _env = env;
            _config = config;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var cancellationToken = context.CancellationToken;

            if (_env.IsDevelopment())
            {
                _logger.LogInformation("Skipping motion detection when running in Development/Windows");
                return;
            }

            var rtspUrl = _config["Motion:RtspUrl"] ?? "rtsp://localhost:8554/cam";
            var outputDir = _config["Motion:OutputDir"] ?? GlobalConstants.MotionRecordingsFolderLinux;
            var recordSeconds = int.TryParse(_config["Motion:RecordSeconds"], out var rs) ? rs : 10;
            var detectionDurationMinutes = int.TryParse(_config["Motion:DetectionDurationMinutes"], out var dm) ? dm : 30;
            var cooldownSeconds = int.TryParse(_config["Motion:CooldownSeconds"], out var cd) ? cd : 30;

            Directory.CreateDirectory(outputDir);

            _logger.LogInformation("Starting motion detection for {minutes} minutes against {url}", detectionDurationMinutes, rtspUrl);

            var detectionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            detectionCts.CancelAfter(TimeSpan.FromMinutes(detectionDurationMinutes));

            try
            {
                await RunDetectionLoop(rtspUrl, outputDir, recordSeconds, cooldownSeconds, detectionCts.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Motion detection cancelled or finished");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Motion detection failed");
            }
        }

        private async Task RunDetectionLoop(string rtspUrl, string outputDir, int recordSeconds, int cooldownSeconds, CancellationToken token)
        {
            // Start ffmpeg to emit MJPEG frames to stdout
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-rtsp_transport tcp -i \"{rtspUrl}\" -vf scale=320:240 -r 2 -f image2pipe -vcodec mjpeg -",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var ffmpeg = System.Diagnostics.Process.Start(startInfo);
            if (ffmpeg == null)
            {
                _logger.LogError("Failed to start ffmpeg for frame grabbing");
                return;
            }

            var stdout = ffmpeg.StandardOutput.BaseStream;
            _ = Task.Run(() => ReadProcessErrorsAsync(ffmpeg, token), token);

            byte[] previousFrame = null;
            int consecutiveMotion = 0;
            var motionCooldownUntil = DateTime.MinValue;

            while (!token.IsCancellationRequested)
            {
                var jpeg = await ReadJpegFromStreamAsync(stdout, token);
                if (jpeg == null) break;

                if (previousFrame != null)
                {
                    bool motion = DetectMotion(previousFrame, jpeg);
                    if (motion)
                    {
                        consecutiveMotion++;
                        _logger.LogDebug("Motion detected (consecutive {count})", consecutiveMotion);
                    }
                    else
                    {
                        consecutiveMotion = 0;
                    }

                    if (consecutiveMotion >= 2 && DateTime.UtcNow > motionCooldownUntil)
                    {
                        // trigger recording
                        var fileName = $"motion-{DateTime.UtcNow:yyyyMMdd_HHmmss}.mp4";
                        var outputPath = Path.Combine(outputDir, fileName);
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await RecordClipAsync(rtspUrl, recordSeconds, outputPath, token);
                                _logger.LogInformation("Saved motion clip to {path}", outputPath);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to record clip");
                            }
                        }, token);

                        motionCooldownUntil = DateTime.UtcNow.AddSeconds(cooldownSeconds);
                        consecutiveMotion = 0;
                    }
                }

                previousFrame = jpeg;
            }

            try
            {
                if (!ffmpeg.HasExited)
                {
                    ffmpeg.Kill(true);
                }
            }
            catch { }
        }

        private async Task ReadProcessErrorsAsync(System.Diagnostics.Process proc, CancellationToken token)
        {
            try
            {
                var sr = proc.StandardError;
                while (!sr.EndOfStream && !token.IsCancellationRequested)
                {
                    var line = await sr.ReadLineAsync();
                    if (!string.IsNullOrEmpty(line))
                        _logger.LogDebug("ffmpeg: {line}", line);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error reading ffmpeg stderr");
            }
        }

        private async Task<byte[]> ReadJpegFromStreamAsync(Stream stream, CancellationToken token)
        {
            var ms = new MemoryStream();
            var buffer = new byte[4096];
            bool started = false;
            int prev = -1;

            while (!token.IsCancellationRequested)
            {
                int read = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                if (read <= 0) break;
                for (int i = 0; i < read; i++)
                {
                    var b = buffer[i];
                    if (!started)
                    {
                        if (prev == 0xFF && b == 0xD8)
                        {
                            started = true;
                            ms.WriteByte(0xFF);
                            ms.WriteByte(0xD8);
                        }
                        prev = b;
                    }
                    else
                    {
                        ms.WriteByte(b);
                        if (prev == 0xFF && b == 0xD9)
                        {
                            return ms.ToArray();
                        }
                        prev = b;
                    }
                }
            }

            return null;
        }

        private bool DetectMotion(byte[] a, byte[] b)
        {
            try
            {
                using var ia = Image.Load<Rgba32>(a);
                using var ib = Image.Load<Rgba32>(b);

                ia.Mutate(x => x.Resize(64, 48).Grayscale());
                ib.Mutate(x => x.Resize(64, 48).Grayscale());

                int width = 64, height = 48;
                int changed = 0;
                for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    var pa = ia[x, y];
                    var pb = ib[x, y];
                    var la = (int)(0.299 * pa.R + 0.587 * pa.G + 0.114 * pa.B);
                    var lb = (int)(0.299 * pb.R + 0.587 * pb.G + 0.114 * pb.B);
                    if (Math.Abs(la - lb) > 20) changed++;
                }

                return changed > 120; // tunable
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to compare frames");
                return false;
            }
        }

        private async Task RecordClipAsync(string rtspUrl, int seconds, string outputPath, CancellationToken token)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-rtsp_transport tcp -y -i \"{rtspUrl}\" -t {seconds} -c copy \"{outputPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = System.Diagnostics.Process.Start(startInfo);
            if (proc == null) throw new InvalidOperationException("Failed to start ffmpeg to record clip");

            var stderr = Task.Run(async () =>
            {
                await ReadProcessErrorsAsync(proc, token);
            }, token);

            await proc.WaitForExitAsync(token);
            await stderr;
        }
    }
}
