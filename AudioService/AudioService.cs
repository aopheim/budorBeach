using System.Diagnostics;
using System.Runtime.InteropServices;
using AudioService.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared;

namespace AudioService;

public class AudioService : IAudioService
{
    private const int MaxNumberOfAudioFiles = 100;
    private static HttpClient? _httpClient;
    private readonly IWebHostEnvironment _environment;
    private readonly IExternalSingletonProcess _externalProcess;
    private readonly IFileSystemService _fileSystemService;
    private readonly bool _isLinux;
    private readonly bool _isWindows;
    private readonly ILogger<AudioService> _logger;
    private readonly TimeSpan MinRecordingLength = TimeSpan.FromSeconds(1);
    private bool _isRunning;
    private bool _shouldExit;

    public AudioService(ILogger<AudioService> logger, IExternalSingletonProcess externalProcess,
        IFileSystemService fileSystemService, IWebHostEnvironment environment)
    {
        _logger = logger;
        _externalProcess = externalProcess;
        _fileSystemService = fileSystemService;
        _environment = environment;
        _isRunning = false;
        _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        _isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        _httpClient = _isWindows ? null : new HttpClient();
        _shouldExit = false;
    }

    public async Task CaptureAudioContinuously(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Starting audio capture... IsWindows? {_isWindows}");
        while (!cancellationToken.IsCancellationRequested && !RecordingFolderIsFull() && !_shouldExit)
        {
            _isRunning = true;
            try
            {
                if (_isWindows) await CaptureAudioWindows(cancellationToken);
                if (_isLinux) await CaptureAudioLinux(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while recording audio");
                _isRunning = false;
                return;
            }
        }

        _isRunning = false;
    }


    public bool IsRunning()
    {
        return _isRunning;
    }

    private async Task CaptureAudioLinux(CancellationToken cancellationToken)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        if (_httpClient == null)
        {
            _logger.LogError("HttpClient not instantiated");
            return;
        }

        var url = _environment.IsDevelopment()
            ? GlobalConstants.AudioRecorderWindowsUrl
            : GlobalConstants.AudioRecorderLinuxUrl;
        _logger.LogInformation($"Posting request to {url}");
        var res = await _httpClient.PostAsync(url, new StringContent(""),
            cancellationToken);
        _logger.LogInformation($"Received status code {res.StatusCode}");
        stopWatch.Stop();
        if (stopWatch.Elapsed < MinRecordingLength)
        {
            _logger.LogInformation($"Recorded shorter than {MinRecordingLength}. Exiting");
            _shouldExit = true;
        }

        _isRunning = false;
    }

    private async Task CaptureAudioWindows(CancellationToken cancellationToken)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var filePath = GlobalConstants.AudioServiceFolderWindows;
        var fileName = "recordAudioWindows.py";
        var argumentsWindows = @$"/C cd {filePath} && py {fileName}";
        _logger.LogInformation("Starting audio capture on Windows...");
        var process = _externalProcess.StartExternalSingletonProcess(_isWindows,
            argumentsWindows, cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        if (stopWatch.Elapsed < MinRecordingLength)
        {
            _logger.LogInformation($"Recorded shorter than {MinRecordingLength}. Exiting");
            _shouldExit = true;
        }

        _isRunning = false;
    }

    private bool RecordingFolderIsFull()
    {
        var audioFiles = _fileSystemService
            .GetFileNamesWithoutExtensionInFolder(_isWindows
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/BudorBeach/audioRecordings"
                : GlobalConstants.AudioRecordingsFolderLinux).ToList();
        var result = audioFiles.Count > MaxNumberOfAudioFiles;
        if (result)
            _logger.LogInformation(
                $"Recording folder is full: Contains {MaxNumberOfAudioFiles} audio files. Exiting audio recorder...");
        return result;
    }
}