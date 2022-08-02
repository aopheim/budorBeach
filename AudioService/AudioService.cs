using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AudioService.Interfaces;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared;

namespace AudioService
{
    public class AudioService : IAudioService
    {
        private readonly IExternalSingletonProcess _externalProcess;
        private readonly ILogger<AudioService> _logger;
        private bool _isRunning;

        public AudioService(ILogger<AudioService> logger, IExternalSingletonProcess externalProcess)
        {
            _logger = logger;
            _externalProcess = externalProcess;
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
                var argumentsLinux = $"-c \"cd {filePath} && python3 {fileName}\"";
                try
                {
                    Process process = _externalProcess.StartExternalSingletonProcess(isWindows,
                        isWindows ? argumentsWindows : argumentsLinux, cancellationToken);
                    await process.WaitForExitAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while recording audio: ");
                    _isRunning = false;
                    continue;
                }

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