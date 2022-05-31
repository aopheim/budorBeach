using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AudioService.Interfaces;
using Microsoft.Extensions.Logging;
using Shared;

namespace AudioService
{
    public class AudioService : IAudioService
    {
        private readonly ILogger<AudioService> _logger;
        private bool _isRunning;

        public AudioService(ILogger<AudioService> logger)
        {
            _logger = logger;
            _isRunning = false;
        }

        public async Task<bool> CaptureAudio(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _isRunning = true;
                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                var filePath = isWindows
                    ? GlobalConstants.AudioServiceFolderWindows
                    : GlobalConstants.AudioServiceFolderLinux;
                var fileName = isWindows ? "recordAudioWindows.py" : "recordAudioLinux.py";
                var argumentsWindows = @$"/C cd {filePath} && py {fileName}";
                var argumentsLinux = $"-c \"cd {filePath} && python {fileName}\"";
                _logger.LogInformation($"Command to run: {argumentsLinux}");
                Process process = new();
                ProcessStartInfo windowsStartInfo = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = argumentsWindows,
                    RedirectStandardOutput = true
                };
                ProcessStartInfo linuxStartInfo = new()
                {
                    FileName = "/bin/bash",
                    Arguments = argumentsLinux,
                    RedirectStandardOutput = true
                };
                process.StartInfo = isWindows ? windowsStartInfo : linuxStartInfo;
                _logger.LogInformation($"Beginning recording using {fileName}");
                process.Start();
                await process.WaitForExitAsync(cancellationToken);
                _logger.LogInformation("Recording finished");

                _isRunning = false;
            }

            return true;
        }

        public bool IsRunning()
        {
            return _isRunning;
        }
    }
}