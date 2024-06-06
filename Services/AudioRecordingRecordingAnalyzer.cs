using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Dtos;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;
using Shared.Models;

namespace Services
{
    public class AudioRecordingRecordingAnalyzer : IBirdRecordingAnalyzer
    {
        private readonly IBirdNetServer _birdNetServer;

        private readonly List<string> _englishNamesToExcludeFromUpload =
            new() { "human", "human vocal", "human non-vocal", "human whistle" };

        private readonly IFileSystemService _fileSystemService;

        private readonly List<string> _latinNamesToExcludeFromUpload =
            new() { "homo sapiens" };

        private readonly ILogger<AudioRecordingRecordingAnalyzer> _logger;
        private readonly IRepositories _repos;
        private readonly IBirdNetResultConverter _resultConverter;
        private readonly IRpiDaemonSettingsService _rpiDaemonSettingsService;
        private readonly ISpeciesNameTranslator _translator;
        private readonly int MaxNumberOfFilesToAnalyze = 15;
        private bool _isRunning;


        public AudioRecordingRecordingAnalyzer(ILogger<AudioRecordingRecordingAnalyzer> logger,
            IBirdNetServer birdNetServer,
            ISpeciesNameTranslator translator, IRepositories repos, IBirdNetResultConverter resultConverter,
            IFileSystemService fileSystemService, IRpiDaemonSettingsService rpiDaemonSettingsService)
        {
            _logger = logger;
            _birdNetServer = birdNetServer;
            _translator = translator;
            _repos = repos;
            _resultConverter = resultConverter;
            _fileSystemService = fileSystemService;
            _rpiDaemonSettingsService = rpiDaemonSettingsService;
            _isRunning = false;
        }

        public double MinConfidenceLevel { get; set; }

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
            MinConfidenceLevel = (await _rpiDaemonSettingsService.GetRpiDaemonSettings(cancellationToken))
                .SpeciesRecognitionConfidence;
            var windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                              @"\BudorBeach\audioRecordings\";
            var linuxPath = GlobalConstants.AudioRecordingsFolderLinux;
            var recordingsFolderName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? windowsPath
                : linuxPath;

            var recordingIds = _fileSystemService.GetFileNamesWithoutExtensionInFolder(recordingsFolderName).ToList();

            var speciesRecognitionsToAddToDb = new List<SpeciesRecognitionModel>();
            var recordingIdsToAnalyze = (recordingIds.Count > MaxNumberOfFilesToAnalyze
                ? recordingIds.Take(
                    MaxNumberOfFilesToAnalyze)
                : recordingIds).ToList();
            var allHiddenSpeciesIds =
                (await _repos.HiddenSpecies.GetAllAsync(cancellationToken)).Select(s => s.TaxonomySpeciesId);

            _logger.LogInformation(
                $"Found {recordingIdsToAnalyze.Count} recordings to analyze. Min confidence set to {MinConfidenceLevel}");
            foreach (var recordingId in recordingIdsToAnalyze)
            {
                if (!Guid.TryParse(recordingId, out var recordingIdAsGuid))
                {
                    _logger.LogWarning(
                        $"Found recording with name {recordingId}, which is not parsable to guid. Skipping");
                    continue;
                }

                var filePath = recordingsFolderName + recordingId + ".wav";
                var existingRecognitionsForRecording =
                    (await _repos.SpeciesRecognitions.WhereAsync(r => r.RecordingId == recordingIdAsGuid,
                        cancellationToken))?.ToList() ?? new List<SpeciesRecognitionModel>();
                if (existingRecognitionsForRecording.Any())
                {
                    _logger.LogInformation($"Recording {recordingIdAsGuid} already analyzed.");
                    if (existingRecognitionsForRecording.Any(r => r.RecordingUploadedAt != null))
                    {
                        _logger.LogWarning($"Recording {recordingIdAsGuid} already uploaded. Deleting recording.");
                        _fileSystemService.DeleteFile(filePath);
                    }

                    continue;
                }

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var response = await _birdNetServer.PostAsync(filePath, cancellationToken);
                stopwatch.Stop();
                _logger.LogInformation(
                    $"Response from BirdNet server in {stopwatch.ElapsedMilliseconds}ms: {response}");

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
                {
                    var speciesId = _translator.GetTaxonomyCodeFromLatinAndEnglishName(dto.LatinName, dto.EnglishName);
                    if (allHiddenSpeciesIds.Contains(speciesId)) return null;
                    return new SpeciesRecognitionModel
                    {
                        Confidence = dto.Confidence,
                        EnglishName = dto.EnglishName,
                        LatinName = dto.LatinName,
                        RecognizedAtUtc = _fileSystemService.GetFileCreationTimeUtc(filePath),
                        RecordingId = recordingIdAsGuid,
                        EBirdTaxonomyId = speciesId
                    };
                }).Where(r => r != null);

                speciesRecognitionsToAddToDb.AddRange(modelsToSave);
            }

            if (speciesRecognitionsToAddToDb.Any())
                await _repos.SpeciesRecognitions.AddRangeAsync(speciesRecognitionsToAddToDb, cancellationToken);
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