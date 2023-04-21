using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;
using Shared.Models;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Services
{
    public class AudioRecordingRecordingAnalyzer : IBirdRecordingAnalyzer
    {
        private readonly IBirdNetServer _birdNetServer;
        private readonly Container _container;

        private readonly List<string> _englishNamesToExcludeFromUpload =
            new() { "human", "human vocal", "human non-vocal", "human whistle" };

        private readonly List<string> _latinNamesToExcludeFromUpload =
            new() { "homo sapiens" };

        private readonly ILogger<AudioRecordingRecordingAnalyzer> _logger;
        private readonly ISpeciesNameTranslator _translator;
        private readonly int MaxNumberOfFilesToAnalyze = 15;
        private bool _isRunning;


        public AudioRecordingRecordingAnalyzer(ILogger<AudioRecordingRecordingAnalyzer> logger,
            IBirdNetServer birdNetServer, Container container,
            ISpeciesNameTranslator translator)
        {
            _logger = logger;
            _birdNetServer = birdNetServer;
            _container = container;
            _translator = translator;
            _isRunning = false;
        }

        public static double MinConfidenceLevel => 0.7;

        public bool IsRunning()
        {
            return _isRunning;
        }

        public async Task RunAnalyzer(CancellationToken cancellationToken)
        {
            _isRunning = true;
            try
            {
                await RunAnalyzerInternal(cancellationToken);
            }
            catch (Exception e)
            {
                _isRunning = false;
                _logger.LogError(e, "Audio recording analyzer failed");
            }

            _isRunning = false;
        }

        private async Task RunAnalyzerInternal(CancellationToken cancellationToken)
        {
            using var scope = AsyncScopedLifestyle.BeginScope(_container);
            var repos = scope.GetRequiredService<IRepositories>();
            var resultConverter = scope.GetRequiredService<IBirdNetResultConverter>();
            var fileSystemService = scope.GetRequiredService<IFileSystemService>();

            var windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                              @"\BudorBeach\audioRecordings\";
            var linuxPath = GlobalConstants.AudioRecordingsFolderLinux;
            var recordingsFolderName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? windowsPath
                : linuxPath;

            var recordingIds = fileSystemService.GetFileNamesWithoutExtensionInFolder(recordingsFolderName).ToList();

            var speciesRecognitionsToAddToDb = new List<SpeciesRecognitionModel>();
            var recordingIdsToAnalyze = (recordingIds.Count > MaxNumberOfFilesToAnalyze
                ? recordingIds.Take(
                    MaxNumberOfFilesToAnalyze)
                : recordingIds).ToList();
            var allHiddenSpeciesIds =
                (await repos.HiddenSpecies.GetAllAsync(cancellationToken)).Select(s => s.TaxonomySpeciesId);
            _logger.LogInformation($"Found {recordingIdsToAnalyze.Count} recordings to analyze");
            foreach (var recordingId in recordingIdsToAnalyze)
            {
                if (!Guid.TryParse(recordingId, out var recordingIdAsGuid)) continue;
                if (repos.SpeciesRecognitions?.Exists(recordingIdAsGuid) ?? false)
                    continue;
                var filePath = recordingsFolderName + recordingId + ".wav";
                var response = await
                    _birdNetServer.PostAsync(filePath,
                        cancellationToken);
                _logger.LogInformation($"Response from server: {response}");

                var result = resultConverter.ConvertJson(response);
                if (!result.Results?.Any(r =>
                        r?.Confidence > MinConfidenceLevel) ?? true)
                {
                    _logger.LogInformation(
                        $"No results with higher confidence than {MinConfidenceLevel}. Deleting recording");
                    fileSystemService.DeleteFile(filePath);
                    continue;
                }

                if (BirdNetOutputContainsSensoredSpecies(result))
                {
                    _logger.LogInformation("Human detected in recording. Deleting.");
                    fileSystemService.DeleteFile(filePath);
                    continue;
                }

                var modelsToSave = result.Results.Where(r => r.Confidence >= MinConfidenceLevel).Select(dto =>
                {
                    var speciesId = _translator.GetTaxonomyCodeFromLatinAndEnglishName(dto.LatinName, dto.EnglishName);
                    if (allHiddenSpeciesIds.Contains(speciesId)) return null;
                    return new SpeciesRecognitionModel
                    {
                        Confidence = dto.Confidence,
                        EnglishName = dto.EnglishName,
                        LatinName = dto.LatinName,
                        // Should be save time for the wav file. Fix later...
                        RecognizedAtUtc = DateTime.UtcNow,
                        RecordingId = recordingIdAsGuid,
                        EBirdTaxonomyId = speciesId
                    };
                }).Where(r => r != null);

                speciesRecognitionsToAddToDb.AddRange(modelsToSave);
            }

            if (speciesRecognitionsToAddToDb.Any())
            {
                await repos.SpeciesRecognitions.AddRangeAsync(speciesRecognitionsToAddToDb, cancellationToken);
                await repos.SaveChangesAsync(cancellationToken);
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