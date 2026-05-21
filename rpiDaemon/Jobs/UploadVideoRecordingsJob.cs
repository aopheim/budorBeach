using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;
using Shared.Models;

namespace rpiDaemon.Jobs;

[DisallowConcurrentExecution]
public class UploadVideoRecordingsJob : IJob
{
    private const int MaxNumberToUpload = 10;
    private readonly IAzureStorageService _azureStorageService;
    private readonly IHostEnvironment _environment;
    private readonly IFileSystemService _fileSystemService;
    private readonly ILogger<UploadVideoRecordingsJob> _logger;
    private readonly IRepositories _repos;

    public UploadVideoRecordingsJob(IAzureStorageService azureStorageService, IFileSystemService fileSystemService,
        ILogger<UploadVideoRecordingsJob> logger, IHostEnvironment environment, IRepositories repos)
    {
        _azureStorageService = azureStorageService;
        _fileSystemService = fileSystemService;
        _logger = logger;
        _environment = environment;
        _repos = repos;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var videoFolder = _environment.IsDevelopment()
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "budorBeach", "videos")
            : GlobalConstants.MotionRecordingsFolderLinux;

        if (!Directory.Exists(videoFolder))
        {
            _logger.LogInformation("Video recordings folder does not exist: {folder}", videoFolder);
            return;
        }

        var videosForUpload = _fileSystemService.GetFileNamesInFolder(videoFolder).ToList();

        if (!videosForUpload.Any())
        {
            _logger.LogInformation("No videos found in {folder}. Exiting.", videoFolder);
            return;
        }

        _logger.LogInformation("Found {Count} files for upload. Uploading the first {Max}", videosForUpload.Count(),
            MaxNumberToUpload);

        foreach (var fileName in videosForUpload.Take(MaxNumberToUpload))
        {
            var uploadTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(300));
            var linkedCToken =
                CancellationTokenSource.CreateLinkedTokenSource(uploadTimeout.Token, context.CancellationToken);
            var localFileLocation = Path.Combine(videoFolder, fileName);

            try
            {
                _logger.LogInformation("Uploading video {fileName} to Azure container", fileName);
                await _azureStorageService.UploadFileFromPath(GlobalConstants.VideoRecordingsContainerName,
                    localFileLocation, fileName, linkedCToken.Token);

                var videoUrl = _azureStorageService.GetBlobUrl(GlobalConstants.VideoRecordingsContainerName, fileName);
                var recordedAtUtc = _fileSystemService.GetFileCreationTimeUtc(localFileLocation);

                await _repos.VideoUploads.AddAsync(new VideoUploadModel
                {
                    FileName = fileName,
                    RecordedAtUtc = recordedAtUtc,
                    VideoUrl = videoUrl
                }, context.CancellationToken);

                _logger.LogInformation("Saved VideoUpload entry {fileName} to VideoUploads repo. Deleting local file",
                    fileName);
                _fileSystemService.DeleteFile(localFileLocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload video {fileName}", fileName);
            }
        }
    }
}
