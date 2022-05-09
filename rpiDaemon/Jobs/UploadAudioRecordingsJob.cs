using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Quartz;
using Services.Interfaces;
using Shared;

namespace rpiDaemon.Jobs
{
    public class UploadAudioRecordingsJob : IJob
    {
        private readonly IAzureStorageService _azureStorageService;
        private readonly IFileSystemService _fileSystemService;

        public UploadAudioRecordingsJob(IFileSystemService fileSystemService, IAzureStorageService azureStorageService)
        {
            _fileSystemService = fileSystemService;
            _azureStorageService = azureStorageService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var recordingsFolderName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BudorBeach\"
                : GlobalConstants.AudioRecordingsFolderLinux;

            var recordingIds = _fileSystemService.GetFileNamesWithoutExtensionInFolder(recordingsFolderName).ToList();

            foreach (var recordingId in recordingIds)
            {
                var filePath = recordingsFolderName + recordingId + ".wav";
                if (!await _azureStorageService.ExistsAsync(GlobalConstants.AudioRecordingsContainerName,
                    $"{recordingId}.wav", context.CancellationToken))
                    await _azureStorageService.UploadFileFromPath(GlobalConstants.AudioRecordingsContainerName,
                        filePath,
                        recordingId + ".wav", context.CancellationToken);

                _fileSystemService.DeleteFile(filePath);
            }
        }
    }
}