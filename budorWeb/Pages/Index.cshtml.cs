using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BirdSpeciesNameTranslator;
using DataAccess.EFCore;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;
using Shared.Models;
using Shared.PiCameraSettings;
using Shared.SignalR;

namespace budorWeb.Pages
{
    public class BudorBeachModel : PageModel
    {
        private const int NumberOfLatestSpeciesRecognitions = 30;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConfiguration _config;
        private readonly BudorDbContext _context;
        private readonly PiCameraSettings _currentCameraSettings;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<BudorBeachModel> _logger;
        private readonly IRepositories _repos;
        private readonly ISignalRService _signalRService;
        private readonly IBirdSpeciesNameTranslator _speciesTranslator;

        public BudorBeachModel(ILogger<BudorBeachModel> logger, IConfiguration config, BudorDbContext context,
            ISignalRService signalRService,
            IRepositories repos, IAzureStorageService azureStorageService, IWebHostEnvironment environment,
            IBirdSpeciesNameTranslator speciesTranslator)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            _context = context;
            _signalRService = signalRService;
            _repos = repos;
            _azureStorageService = azureStorageService;
            _environment = environment;
            _speciesTranslator = speciesTranslator;
            _logger = logger;
            _config = config;
            _currentCameraSettings = PiCameraSettingsHelper.GetCurrentCameraSettingsFromFile();
            IsoSetting = _currentCameraSettings.Iso;
            ShutterTimeSetting = _currentCameraSettings.ShutterTime;
            SetupWebClientMethods();
        }

        private PiCameraSettings CurrentCameraSettings { get; }
        public List<ImageDto> LatestImages { get; set; }
        public List<SpeciesRecognitionDto> LatestSpeciesRecognitions { get; set; }
        [CanBeNull] public SensorReadingModel LatestSensorReadingModel { get; set; }
        [BindProperty] public int IsoSetting { get; set; }
        [BindProperty] public int ShutterTimeSetting { get; set; }

        private void SetupWebClientMethods()
        {
            _signalRService.RegisterClientMethod<PiCameraSettings>(nameof(ISignalRService.TakeImage),
                settings => { _logger.LogInformation(JsonSerializer.Serialize(settings)); });
            _signalRService.RegisterClientMethod<SensorReadingModel>(nameof(ISignalRService.SendSensorReading),
                model => { _logger.LogInformation(JsonSerializer.Serialize(model)); });
        }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            // Adding log to see if App Insights logs it
            _logger.LogInformation("From OnGetAsync");
            await _signalRService.ConsoleLogMessage(".NET Web Client connected!", cancellationToken);

            var latestTime = _context.SensorReadings.Max(s => s.MeasuredAtUtc);
            LatestSensorReadingModel = _context.SensorReadings.FirstOrDefault(s => s.MeasuredAtUtc == latestTime);
            LatestImages = _repos.ImageUploads.GetLatestUploads(6).Select(iu => new ImageDto
                    { Name = iu.FileName, ImageUrl = iu.FullSizeImageUrl, ThumbnailUrl = iu.ThumbnailWebPImageUrl })
                .ToList();

            var latestRecognitions =
                _repos.SpeciesRecognitions.GetLatestRecognitions(NumberOfLatestSpeciesRecognitions);
            LatestSpeciesRecognitions = new List<SpeciesRecognitionDto>();
            foreach (var r in latestRecognitions)
            {
                var fileNameWithExtension = $"{r.RecordingId}.wav";
                var recordingExist = await _azureStorageService.ExistsAsync(
                    GlobalConstants.AudioRecordingsContainerName,
                    fileNameWithExtension, cancellationToken);
                LatestSpeciesRecognitions.Add(new SpeciesRecognitionDto
                {
                    Confidence = r.Confidence,
                    RecordingUrl = recordingExist
                        ? _environment.IsDevelopment()
                            ? @$"https://127.0.0.1:10000/devstoreaccount1/{GlobalConstants.AudioRecordingsContainerName}/{fileNameWithExtension}"
                            : @$"https://budorbeach.blob.core.windows.net/{GlobalConstants.AudioRecordingsContainerName}/{fileNameWithExtension}"
                        : null,
                    LatinSpeciesName = r.LatinName,
                    NorwegianSpeciesName = _speciesTranslator.TranslateFromLatinName(r.LatinName),
                    EnglishSpeciesName = r.EnglishName,
                    SpeciesId = _speciesTranslator.GetTaxonomyCodeFromLatinAndEnglishName(r.LatinName, r.EnglishName),
                    RecognizedAtUtc = r.RecognizedAtUtc,
                    ThumbnailSpeciesImageUrl =
                        "https://upload.wikimedia.org/wikipedia/commons/7/77/Ficedula_hypoleuca_G%C3%B6teborg_2.jpg"
                });
            }
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            PiCameraSettingsHelper.SetCameraSettingsToFile(new PiCameraSettings(IsoSetting, ShutterTimeSetting));
            await _signalRService.TakeImage(CurrentCameraSettings, cancellationToken);

            return RedirectToPage("Index");
        }

        public JsonResult OnGetSensorReadings(int cutOffHours)
        {
            var sensorReadingCutOff = DateTime.UtcNow.AddHours(-cutOffHours);
            var sensorReadings = _context.SensorReadings.Where(m => m.MeasuredAtUtc >= sensorReadingCutOff).ToList()
                .OrderBy(m => m.MeasuredAtUtc).ToList();

            // Show at least one point every hour if showing data for a full year
            var filteredSensorReadings = new List<SensorReadingModel>();
            const int maxElementsToShow = 24 * 365;
            if (sensorReadings.Count > maxElementsToShow)
            {
                var keepEveryNth = sensorReadings.Count / maxElementsToShow;
                for (var i = 0; i < sensorReadings.Count; i++)
                    if (i % keepEveryNth == 0)
                        filteredSensorReadings.Add(sensorReadings[i]);
            }

            if (!filteredSensorReadings.Any()) filteredSensorReadings = sensorReadings;

            var chartModel = new SensorReadingsChartDto
            {
                HumidityReadings = filteredSensorReadings.Select(m => Math.Round(m.RelativeHumidityInPercent, 2))
                    .ToList(),
                TemperatureReadings =
                    filteredSensorReadings.Select(m => Math.Round(m.TemperatureInDegreesC, 2)).ToList(),
                PressureReadings = filteredSensorReadings.Select(m => Math.Round(m.PressureInhPa, 2)).ToList(),
                MeasuredAt = filteredSensorReadings.Select(m => m.MeasuredAtUtc.ToLocalTime())
                    .ToList()
            };
            return new JsonResult(chartModel);
        }
    }

    public class SensorReadingsChartDto
    {
        public List<double> TemperatureReadings { get; set; }
        public List<double> PressureReadings { get; set; }
        public List<double> HumidityReadings { get; set; }
        public List<DateTime> MeasuredAt { get; set; }
    }

    public class ImageDto
    {
        public string ThumbnailUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
    }

    public class SpeciesRecognitionDto
    {
        public Guid Id { get; set; }
        public DateTime RecognizedAtUtc { get; set; }
        public string LatinSpeciesName { get; set; }
        public string NorwegianSpeciesName { get; set; }
        public string EnglishSpeciesName { get; set; }
        public string SpeciesId { get; set; }
        public double Confidence { get; set; }
        public string ThumbnailSpeciesImageUrl { get; set; }
        [CanBeNull] public string RecordingUrl { get; set; }
    }
}