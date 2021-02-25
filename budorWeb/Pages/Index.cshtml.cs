using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using rpiDaemon;
using Shared;
using Shared.Interfaces;
using Shared.Models;
using Shared.PiCameraSettings;
using Shared.SignalR;

namespace budorWeb.Pages
{
    public class BudorBeachModel : PageModel
    {
        private const string BlobContainerName = "images";
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly HubConnection _connection;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BudorBeachModel> _logger;
        private List<SensorReadingModel> _sensorReadingsFromLastMonth;
        private List<SensorReadingModel> _sensorReadingsFromLastSevenDays;

        public BudorBeachModel(ILogger<BudorBeachModel> logger, IConfiguration config, ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _cloudBlobClient = CloudStorageAccount.Parse(config.GetConnectionString("AzureStorageConnectionString"))
                .CreateCloudBlobClient();
            _currentCameraSettings = PiCameraSettingsHelper.GetCurrentCameraSettingsFromFile();
            IsoSetting = _currentCameraSettings.Iso;
            ShutterTimeSetting = _currentCameraSettings.ShutterTime;

            const string developmentUrl = "http://localhost:3000/budorhub";
            const string productionUrl = "https://budorbeach.azurewebsites.net/budorhub";
            _connection = SignalRHelper.GetHubConnection(environment.IsProduction() ? productionUrl : developmentUrl);
            SignalRHelper.SetupEventsForDebuggingConnection(logger, _connection);
        }

        private PiCameraSettings _currentCameraSettings { get; }


        public List<CloudBlockBlob> AllImagesInBlob { get; set; }
        [CanBeNull] public SensorReadingModel LatestSensorReadingModel { get; set; }

        [NotNull]
        public List<SensorReadingModel> SensorReadingsFromLastSevenDays
        {
            get => _sensorReadingsFromLastSevenDays ??= new List<SensorReadingModel>();
            set => _sensorReadingsFromLastSevenDays = value;
        }

        [NotNull]
        public List<SensorReadingModel> SensorReadingsFromLastMonth
        {
            get => _sensorReadingsFromLastMonth ??= new List<SensorReadingModel>();
            set => _sensorReadingsFromLastMonth = value;
        }

        [BindProperty] public int IsoSetting { get; set; }
        [BindProperty] public int ShutterTimeSetting { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            SetupWebClientMethods();
            await SignalRHelper.StartWithRetryAsync(_connection, cancellationToken);
            await _connection.InvokeAsync(nameof(BudorHub.SendMessageToAllClients), ".NET Web Client connected!",
                cancellationToken);


            var now = DateTime.UtcNow;
            var sevenDaysAgo = now.AddDays(-7);
            var oneMonthAgo = now.AddMonths(-1);
            SensorReadingsFromLastSevenDays =
                await _context.SensorReadings.Where(model => model.MeasuredAtUtc > sevenDaysAgo)
                    .ToListAsync(cancellationToken);
            SensorReadingsFromLastMonth = await _context.SensorReadings
                .Where(model => model.MeasuredAtUtc > oneMonthAgo).ToListAsync(cancellationToken);
            LatestSensorReadingModel = _context.SensorReadings.OrderByDescending(m => m.MeasuredAtUtc).FirstOrDefault();

            var blobContainer = _cloudBlobClient.GetContainerReference(BlobContainerName);
            BlobContinuationToken continuationToken = null;
            var allImages = new List<CloudBlockBlob>();

            do
            {
                var response = await blobContainer.ListBlobsSegmentedAsync(default, true, default,
                    default, continuationToken, default, default, cancellationToken);
                continuationToken = response.ContinuationToken;
                allImages.AddRange(response.Results.Cast<CloudBlockBlob>());
            } while (continuationToken != null);

            AllImagesInBlob = allImages;
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            await SignalRHelper.StartWithRetryAsync(_connection, cancellationToken);

            _currentCameraSettings.Iso = IsoSetting;
            _currentCameraSettings.ShutterTime = ShutterTimeSetting;
            PiCameraSettingsHelper.SetCameraSettingsToFile(_currentCameraSettings);
            await _connection.InvokeAsync(nameof(BudorHub.TakeImage), _currentCameraSettings, cancellationToken);

            return RedirectToPage("Index");
        }

        private void SetupWebClientMethods()
        {
            _connection.On<string>(nameof(IBudorHubClient.TakeImage), message => { _logger.LogInformation(message); });
            _connection.On<SensorReadingModel>(nameof(IBudorHubClient.ReceiveCurrentSensorReading),
                model => { _logger.LogInformation(JsonSerializer.Serialize(model)); });
        }

        public string GetLocalDateTimeAsString(DateTime? dateTimeInUtc)
        {
            if (dateTimeInUtc == null)
                return "";
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeInUtc.Value, timeZoneInfo);
            return
                $"{localTime.Day}.{localTime.Month}.{localTime.Year}, {localTime.Hour}:{localTime.Minute}:{localTime.Second}";
        }
    }
}