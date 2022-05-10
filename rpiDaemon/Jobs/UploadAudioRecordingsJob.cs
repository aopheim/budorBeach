using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.Interfaces;
using Shared;

namespace rpiDaemon.Jobs
{
    public class UploadAudioRecordingsJob : IJob
    {
        private readonly IAzureStorageService _azureStorageService;
        private readonly IFileSystemService _fileSystemService;
        private readonly ILogger<UploadAudioRecordingsJob> _logger;

        public UploadAudioRecordingsJob(IFileSystemService fileSystemService, IAzureStorageService azureStorageService,
            ILogger<UploadAudioRecordingsJob> logger)
        {
            _fileSystemService = fileSystemService;
            _azureStorageService = azureStorageService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var recordingsFolderName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BudorBeach\"
                : GlobalConstants.AudioRecordingsFolderLinux;

            var recordingIds = _fileSystemService.GetFileNamesWithoutExtensionInFolder(recordingsFolderName).ToList();

            _logger.LogInformation($"Found {recordingIds.Count} recordings for upload");
            foreach (var recordingId in recordingIds)
            {
                var fileName = recordingId + ".wav";
                var filePath = recordingsFolderName + fileName;
                if (!await _azureStorageService.ExistsAsync(GlobalConstants.AudioRecordingsContainerName,
                    fileName, context.CancellationToken))
                {
                    _logger.LogInformation($"Uploading recording {recordingId}.wav...");
                    await _azureStorageService.UploadFileFromPath(GlobalConstants.AudioRecordingsContainerName,
                        filePath,
                        fileName, context.CancellationToken);
                }

                _logger.LogInformation($"Deleting recording {recordingId}.wav");
                _fileSystemService.DeleteFile(filePath);
            }
        }
    }
}