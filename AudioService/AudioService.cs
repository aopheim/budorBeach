using System.Runtime.InteropServices;
using AudioService.Interfaces;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared;

namespace AudioService;

public class AudioService : IAudioService
{
    private const int MaxNumberOfAudioFiles = 100;
    private readonly IExternalSingletonProcess _externalProcess;
    private readonly IFileSystemService _fileSystemService;
    private readonly bool _isWindows;
    private readonly ILogger<AudioService> _logger;
    private bool _isRunning;

    public AudioService(ILogger<AudioService> logger, IExternalSingletonProcess externalProcess,
        IFileSystemService fileSystemService)
    {
        _logger = logger;
        _externalProcess = externalProcess;
        _fileSystemService = fileSystemService;
        _isRunning = false;
        _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    public async Task<bool> CaptureAudio(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && !RecordingFolderIsFull())
        {
            _isRunning = true;
            var filePath = _isWindows
                ? GlobalConstants.AudioServiceFolderWindows
                : GlobalConstants.AudioServiceFolderLinux;
            var fileName = _isWindows ? "recordAudioWindows.py" : "recordAudioLinux.py";
            var argumentsWindows = @$"/C cd {filePath} && py {fileName}";
            var argumentsLinux = $"-c \"cd {filePath} && python3.9 {fileName}\"";
            try
            {
                var process = _externalProcess.StartExternalSingletonProcess(_isWindows,
                    _isWindows ? argumentsWindows : argumentsLinux, cancellationToken);
                await process.WaitForExitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while recording audio");
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

    private bool RecordingFolderIsFull()
    {
        return _fileSystemService
            .GetFileNamesWithoutExtensionInFolder(_isWindows
                ? GlobalConstants.AudioRecordingsFolderWindows
                : GlobalConstants.AudioRecordingsFolderLinux).ToList().Count > MaxNumberOfAudioFiles;
    }
}