using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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
        private readonly CloudBlobClient _cloudBlobClient;

        private readonly IConfiguration _config;

        private readonly HubConnection _connection;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<BudorBeachModel> _logger;
        private readonly string BlobContainerName = "images";

        public BudorBeachModel(ILogger<BudorBeachModel> logger, IConfiguration config, ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _config = config;
            _logger = logger;
            _cloudBlobClient = CloudStorageAccount.Parse(_config.GetConnectionString("AzureStorageConnectionString"))
                .CreateCloudBlobClient();
            _currentCameraSettings = PiCameraSettingsHelper.GetCurrentCameraSettingsFromFile();

            var developmentUrl = "http://localhost:3000/budorhub";
            var productionUrl = "https://budorbeach.azurewebsites.net/budorhub";
            _connection = new HubConnectionBuilder()
                .WithUrl(environment.IsProduction() ? productionUrl : developmentUrl)
                .WithAutomaticReconnect()
                .Build();
            _connection.Closed += async e =>
            {
                _logger.LogError(e, e.Message);
                await Task.Delay(200);
                await _connection.StartAsync();
            };
            _connection.Reconnecting += e =>
            {
                _logger.LogError(e, e.Message);
                Debug.Assert(_connection.State == HubConnectionState.Reconnecting);
                return Task.CompletedTask;
            };
            _connection.Reconnected += message =>
            {
                _logger.LogInformation(message);
                Debug.Assert(_connection.State == HubConnectionState.Connected);
                return Task.CompletedTask;
            };
        }

        private PiCameraSettings _currentCameraSettings { get; }


        public List<CloudBlockBlob> AllImagesInBlob { get; set; }
        public SensorReadingModel LatestSensorReadingModel { get; set; }
        public List<SensorReadingModel> AllSensorReadings { get; set; }
        public List<SensorReadingModel> SensorReadingsFromLastSevenDays { get; set; }
        public List<SensorReadingModel> SensorReadingsFromLastMonth { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            SetupWebClientMethods();
            await SignalRHelper.ConnectWithRetryAsync(_connection, cancellationToken);
            await _connection.InvokeAsync(nameof(BudorHub.SendMessageToAllClients), ".NET Client connected!",
                cancellationToken);

            // TODO: Set camera settings from UI
            PiCameraSettingsHelper.SetCameraSettingsToFile(_currentCameraSettings);
            await _connection.InvokeAsync(nameof(BudorHub.TakeImage), _currentCameraSettings, cancellationToken);

            var now = DateTime.UtcNow;
            var sevenDaysAgo = now.AddDays(-7);
            var oneMonthAgo = now.AddMonths(-1);
            AllSensorReadings = await _context.SensorReadings.Where(model => true).ToListAsync(cancellationToken);
            SensorReadingsFromLastSevenDays =
                await _context.SensorReadings.Where(model => model.MeasuredAtUtc > sevenDaysAgo)
                    .ToListAsync(cancellationToken);
            SensorReadingsFromLastMonth = await _context.SensorReadings
                .Where(model => model.MeasuredAtUtc > oneMonthAgo).ToListAsync(cancellationToken);

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

        private void SetupWebClientMethods()
        {
            _connection.On<string>(nameof(IBudorHubClient.TakeImage), message => { _logger.LogInformation(message); });
            _connection.On<SensorReadingModel>(nameof(IBudorHubClient.ReceiveCurrentSensorReading),
                model =>
                {
                    _logger.LogInformation(JsonSerializer.Serialize(model));
                    LatestSensorReadingModel = model;
                });
        }
    }
}