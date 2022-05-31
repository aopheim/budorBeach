using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CameraService
{
    public partial class CameraService
    {
        private readonly CancellationTokenSource _streamingTimeOut = new(TimeSpan.FromHours(2));

        public async Task StartVideoStream(CancellationToken cancellationToken)
        {
            if (_isWindows)
            {
                _logger.LogInformation("Running on Windows. Mocking video stream...");
                return;
            }

            var arguments =
                "-c \"raspivid -o - -t 0 -w 800 -h 600 -fps 12  | cvlc -vvv stream:///dev/stdin --sout '#rtp{sdp = rtsp://:80/}' :demux=h264\"";
            Process process = new();
            ProcessStartInfo startInfo = new()
            {
                FileName = "/bin/bash",
                Arguments = arguments,
                RedirectStandardOutput = true
            };
            process.StartInfo = startInfo;
            _logger.LogInformation("Beginning stream to port 80...");
            process.Start();
            await process.WaitForExitAsync(
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _streamingTimeOut.Token).Token);
        }
    }
}