using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Dtos;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;
using Shared.Models;

namespace rpiDaemon.Jobs
{
    public class AnalyzeAudioRecordingsJob : IJob
    {
        private readonly IAzureStorageService _azureStorageService;
        private readonly IBirdNetServer _birdNetServer;

        private readonly List<string> _englishNamesToExcludeFromUpload =
            new() { "human" };

        private readonly IFileSystemService _fileSystemService;

        private readonly List<string> _latinNamesToExcludeFromUpload =
            new() { "homo sapiens" };

        private readonly ILogger<AnalyzeAudioRecordingsJob> _logger;
        private readonly IRepositories _repos;
        private readonly IBirdNetResultConverter _resultConverter;
        private readonly int MaxNumberOfFilesToAnalyze = 15;

        public AnalyzeAudioRecordingsJob(ILogger<AnalyzeAudioRecordingsJob> logger,
            IBirdNetResultConverter resultConverter, IFileSystemService fileSystemService, IRepositories repos,
            IBirdNetServer birdNetServer, IAzureStorageService azureStorageService)
        {
            _logger = logger;
            _resultConverter = resultConverter;
            _fileSystemService = fileSystemService;
            _repos = repos;
            _birdNetServer = birdNetServer;
            _azureStorageService = azureStorageService;
        }

        private static double MinConfidenceLevel => 0.5;

        public async Task Execute(IJobExecutionContext context)
        {
            var recordingsFolderName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BudorBeach\"
                : GlobalConstants.AudioRecordingsFolderLinux;
            var recordingIds = _fileSystemService.GetFileNamesWithoutExtensionInFolder(recordingsFolderName).ToList();

            var speciesRecognitionsToAddToDb = new List<SpeciesRecognitionModel>();
            var recordingIdsToAnalyze = recordingIds.Count() > MaxNumberOfFilesToAnalyze
                ? recordingIds.Take(
                    MaxNumberOfFilesToAnalyze)
                : recordingIds;
            foreach (var recordingId in recordingIdsToAnalyze)
            {
                if (!Guid.TryParse(recordingId, out var recordingIdAsGuid)) continue;
                if (_repos.SpeciesRecognitions?.Exists(recordingIdAsGuid) ?? false)
                    continue;
                var filePath = recordingsFolderName + recordingId + ".wav";
                var response =
                    await _birdNetServer.PostAsync(filePath,
                        context?.CancellationToken ?? new CancellationToken());
                _logger.LogInformation($"Response from server: {response}");

                var result = _resultConverter.ConvertJson(response);
                if (!result.Results?.Any(r =>
                    r?.Confidence > MinConfidenceLevel) ?? true)
                {
                    _logger.LogInformation(
                        $"No results with higher confidence than {MinConfidenceLevel}. Deleting recording");
                    _fileSystemService.DeleteFile(filePath);
                    continue;
                }

                if (BirdNetOutputContainsSensoredSpecies(result))
                {
                    _logger.LogInformation("Human detected in recording. Deleting.");
                    _fileSystemService.DeleteFile(filePath);
                    continue;
                }

                var modelsToSave = result.Results.Where(r => r.Confidence >= MinConfidenceLevel).Select(dto =>
                    new SpeciesRecognitionModel
                    {
                        Confidence = dto.Confidence,
                        EnglishName = dto.EnglishName,
                        LatinName = dto.LatinName,
                        // Should be save time for the wav file. Fix later...
                        RecognizedAtUtc = DateTime.UtcNow,
                        RecordingId = recordingIdAsGuid
                    });

                speciesRecognitionsToAddToDb.AddRange(modelsToSave);
            }

            if (speciesRecognitionsToAddToDb.Any())
            {
                _repos.SpeciesRecognitions?.AddRange(speciesRecognitionsToAddToDb);
                await _repos.SaveChangesAsync();
            }
        }

        private bool BirdNetOutputContainsSensoredSpecies(BirdNetOutputDto dto)
        {
            return dto.Results?.Any(item =>
                       _latinNamesToExcludeFromUpload.Contains(item.LatinName?.ToLower())
                       || _englishNamesToExcludeFromUpload.Contains(item.EnglishName
                           ?.ToLower())) ??
                   true;
        }
    }
}