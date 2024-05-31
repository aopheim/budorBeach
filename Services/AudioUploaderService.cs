using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;
using Shared.Models;

namespace Services
{
    public class AudioUploaderService : IAudioUploader
    {
        private const int MaxNumberOfFilesToUploadInOneRun = 10;
        public const int MaxUploadsIn24Hrs = 200;
        public const int MaxAudioRecordingsPerSpecies = 500;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IFileSystemService _fileSystemService;
        private readonly ILogger<AudioUploaderService> _logger;
        private readonly IRepositories _repos;
        private bool _isRunning;

        public AudioUploaderService(ILogger<AudioUploaderService> logger, IRepositories repos,
            IAzureStorageService azureStorageService, IFileSystemService fileSystemService)
        {
            _logger = logger;
            _repos = repos;
            _azureStorageService = azureStorageService;
            _fileSystemService = fileSystemService;
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
            var recordingsFolderName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BudorBeach\"
                : GlobalConstants.AudioRecordingsFolderLinux;
            var localRecordingIds = GetLocalRecordingIds(recordingsFolderName);

            var numberOfRecordingsUploadedLast24Hours =
                (await _repos.SpeciesRecognitions.GetSpeciesRecognitionsUploadedSince(DateTime.UtcNow.AddHours(-24),
                    cancellationToken))?.Count() ?? 0;
            var recognitionsForPotentialUpload =
                (await _repos.SpeciesRecognitions.WhereAsync(
                    srm => srm.RecordingUploadedAt == null && localRecordingIds.Contains(srm.RecordingId),
                    cancellationToken)).Take(Math.Max(MaxUploadsIn24Hrs - numberOfRecordingsUploadedLast24Hours, 0));

            var recognitionsForPotentialUploadByRecordingId =
                recognitionsForPotentialUpload.GroupBy(r => r.RecordingId).ToDictionary(g => g.Key, g => g.ToList());
            _logger.LogInformation(
                $"Found {recognitionsForPotentialUploadByRecordingId.Count} potential recordings for upload. Taking {MaxNumberOfFilesToUploadInOneRun}.");
            foreach (var (recordingId, recognitionsInRecording) in recognitionsForPotentialUploadByRecordingId.Take(
                         MaxNumberOfFilesToUploadInOneRun))
            {
                var fileName = recordingId + ".wav";
                var filePath = recordingsFolderName + fileName;
                var uploadRecording = false;
                foreach (var recognition in recognitionsInRecording)
                {
                    if (uploadRecording) break;
                    uploadRecording = await ShouldUploadRecording(recognition, cancellationToken);
                }

                if (!uploadRecording)
                {
                    DeleteRecording(filePath);
                    continue;
                }

                if (!await _azureStorageService.ExistsAsync(GlobalConstants.AudioRecordingsContainerName,
                        fileName, cancellationToken))
                {
                    _logger.LogInformation($"Uploading recording {recordingId}.wav...");
                    await _azureStorageService.UploadFileFromPath(GlobalConstants.AudioRecordingsContainerName,
                        filePath,
                        fileName, cancellationToken);
                    var uploadedAt = DateTime.UtcNow;
                    recognitionsInRecording.ForEach(r => r.RecordingUploadedAt = uploadedAt);
                    await _repos.SpeciesRecognitions.UpdateRangeAsync(recognitionsInRecording, cancellationToken);
                }

                DeleteRecording(filePath);
            }
        }

        private void DeleteRecording(string filePath)
        {
            _logger.LogInformation($"Deleting local recording {filePath}");
            _fileSystemService.DeleteFile(filePath);
        }

        private async Task<bool> ShouldUploadRecording(SpeciesRecognitionModel recognition,
            CancellationToken cancellationToken)
        {
            var uploadedRecognitionsForSpecies =
                (await _repos.SpeciesRecognitions.GetUploadedRecognitionsForEBirdSpeciesId(recognition.EBirdTaxonomyId,
                    MaxAudioRecordingsPerSpecies,
                    cancellationToken)).ToList();
            
            if (!uploadedRecognitionsForSpecies.Any() || (recognition.Confidence > (uploadedRecognitionsForSpecies?.Min(r => r.Confidence) ?? 0))) return true;
            return false;
        }

        private List<Guid> GetLocalRecordingIds(string recordingsFolderName)
        {
            var localRecordingIdsAsString =
                _fileSystemService.GetFileNamesWithoutExtensionInFolder(recordingsFolderName).ToList();
            var localRecordingIds = new List<Guid>();
            foreach (var stringId in localRecordingIdsAsString)
                if (Guid.TryParse(stringId, out var idAsGuid))
                    localRecordingIds.Add(idAsGuid);

            return localRecordingIds;
        }
    }
}