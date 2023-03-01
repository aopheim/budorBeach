using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DataAccess.EFCore;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Interfaces;
using Shared.Models;
using Shared.PiCameraSettings;
using Shared.SignalR;

namespace budorWeb.Pages
{
    public class BudorBeachModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly BudorDbContext _context;
        private readonly PiCameraSettings _currentCameraSettings;
        private readonly ILogger<BudorBeachModel> _logger;
        private readonly IRepositories _repos;
        private readonly ISignalRService _signalRService;

        public BudorBeachModel(ILogger<BudorBeachModel> logger, IConfiguration config, BudorDbContext context,
            ISignalRService signalRService,
            IRepositories repos)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            _context = context;
            _signalRService = signalRService;
            _repos = repos;
            _logger = logger;
            _config = config;
            _currentCameraSettings = PiCameraSettingsHelper.GetCurrentCameraSettingsFromFile();
            IsoSetting = _currentCameraSettings.Iso;
            ShutterTimeSetting = _currentCameraSettings.ShutterTime;
            SetupWebClientMethods();
        }

        private PiCameraSettings CurrentCameraSettings { get; }


        public List<ImageDto> LatestImages { get; set; }
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
            await _signalRService.ConsoleLogMessage(".NET Web Client connected!", cancellationToken);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _logger.LogInformation($"Before SensorReading SQL: {stopwatch.Elapsed.TotalMilliseconds} ms");
            var latestTime = _context.SensorReadings.Max(s => s.MeasuredAtUtc);
            LatestSensorReadingModel = _context.SensorReadings.FirstOrDefault(s => s.MeasuredAtUtc == latestTime);
            _logger.LogInformation($"After SensorReading SQL: {stopwatch.Elapsed.TotalMilliseconds} ms");
            _logger.LogInformation($"Before ImageUpload SQL: {stopwatch.Elapsed.TotalMilliseconds} ms");
            LatestImages = _repos.ImageUploads.GetLatestUploads(6).Select(iu => new ImageDto
                    { Name = iu.FileName, ImageUrl = iu.FullSizeImageUrl, ThumbnailUrl = iu.ThumbnailWebPImageUrl })
                .ToList();
            _logger.LogInformation($"After ImageUpload SQL: {stopwatch.Elapsed.TotalMilliseconds} ms");
            stopwatch.Stop();
            _logger.LogInformation($"Performed OnGetAsync in total {stopwatch.Elapsed.TotalMilliseconds} ms");
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
}