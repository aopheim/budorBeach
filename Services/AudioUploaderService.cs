using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Services
{
    public class AudioUploaderService : IAudioUploader
    {
        private const int MaxNumberOfFilesToUpload = 10;
        private readonly Container _container;
        private readonly ILogger<AudioUploaderService> _logger;
        private bool _isRunning;

        public AudioUploaderService(ILogger<AudioUploaderService> logger, Container container)
        {
            _logger = logger;
            _container = container;
            _isRunning = false;
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public async Task StartUpload(CancellationToken cancellationToken)
        {
            _isRunning = true;
            using var scope = AsyncScopedLifestyle.BeginScope(_container);
            var repos = scope.GetRequiredService<IRepositories>();
            var azureStorageService = scope.GetRequiredService<IAzureStorageService>();
            var fileSystemService = scope.GetRequiredService<IFileSystemService>();

            var recordingsFolderName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BudorBeach\"
                : GlobalConstants.AudioRecordingsFolderLinux;

            var localRecordingIdsAsString =
                fileSystemService.GetFileNamesWithoutExtensionInFolder(recordingsFolderName).ToList();
            var localRecordingIds = new List<Guid>();
            foreach (var stringId in localRecordingIdsAsString)
                if (Guid.TryParse(stringId, out var idAsGuid))
                    localRecordingIds.Add(idAsGuid);

            var localRecordingsToUpload =
                repos.SpeciesRecognitions.Find(srm => localRecordingIds.Contains(srm.RecordingId))
                    .Select(r => r.RecordingId).ToList();
            _logger.LogInformation($"Found {localRecordingsToUpload.Count} recordings for upload");
            foreach (var recordingId in localRecordingsToUpload.Take(MaxNumberOfFilesToUpload))
            {
                var fileName = recordingId + ".wav";
                var filePath = recordingsFolderName + fileName;
                if (!await azureStorageService.ExistsAsync(GlobalConstants.AudioRecordingsContainerName,
                    fileName, cancellationToken))
                {
                    _logger.LogInformation($"Uploading recording {recordingId}.wav...");
                    await azureStorageService.UploadFileFromPath(GlobalConstants.AudioRecordingsContainerName,
                        filePath,
                        fileName, cancellationToken);
                }

                _logger.LogInformation($"Deleting local recording {recordingId}.wav");
                fileSystemService.DeleteFile(filePath);
            }

            _isRunning = false;
        }
    }
}