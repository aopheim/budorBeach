using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Shared;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared.Interfaces;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Services
{
    public class AudioUploaderService : IAudioUploader
    {
        private const int MaxNumberOfFilesToUpload = 10;
        public const int MaxUploadsIn24Hrs = 200;
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
            try
            {
                await StartUploadInternal(cancellationToken);
            }
            catch (Exception e)
            {
                _isRunning = false;
                _logger.LogError(e, "Uploading audio files failed");
            }

            _isRunning = false;
        }

        private async Task StartUploadInternal(CancellationToken cancellationToken)
        {
            await using var scope = AsyncScopedLifestyle.BeginScope(_container);
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

            var numberOfRecordingsUploadedLast24Hours = (await repos.SpeciesRecognitions.WhereAsync(srm =>
                    srm.RecordingUploadedAt != null && srm.RecordingUploadedAt > DateTime.UtcNow.AddHours(-24),
                cancellationToken)).Count();
            var localRecordingsToUpload =
                (await repos.SpeciesRecognitions.WhereAsync(srm => localRecordingIds.Contains(srm.RecordingId),
                    cancellationToken)).Take(Math.Max(MaxUploadsIn24Hrs - numberOfRecordingsUploadedLast24Hours, 0))
                .ToList();

            _logger.LogInformation($"Found {localRecordingsToUpload.Count} recordings for upload");
            foreach (var recording in localRecordingsToUpload.Take(MaxNumberOfFilesToUpload))
            {
                var fileName = recording.RecordingId + ".wav";
                var filePath = recordingsFolderName + fileName;
                if (!await azureStorageService.ExistsAsync(GlobalConstants.AudioRecordingsContainerName,
                    fileName, cancellationToken))
                {
                    _logger.LogInformation($"Uploading recording {recording.RecordingId}.wav...");
                    await azureStorageService.UploadFileFromPath(GlobalConstants.AudioRecordingsContainerName,
                        filePath,
                        fileName, cancellationToken);
                    recording.RecordingUploadedAt = DateTime.UtcNow;
                    await repos.SpeciesRecognitions.UpdateAsync(recording, cancellationToken);
                }

                _logger.LogInformation($"Deleting local recording {recording}.wav");
                fileSystemService.DeleteFile(filePath);
            }
        }
    }
}