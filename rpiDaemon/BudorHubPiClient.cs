using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MMALSharp;
using rpiDaemon.Jobs;
using Shared.Interfaces;
using Shared.Models;
using Shared.PiCameraSettings;
using Shared.SignalR;

namespace rpiDaemon
{
    public class BudorHubPiClient : IBudorHubClient, IHostedService
    {
        private const string BlobContainerName = "images";
        private readonly MMALCamera _camera;

        private readonly HubConnection _connection;
        private readonly BlobContainerClient _containerClient;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<BudorHubPiClient> _logger;

        public BudorHubPiClient(ILogger<BudorHubPiClient> logger, IWebHostEnvironment environment,
            IConfiguration config)
        {
            _logger = logger;
            _environment = environment;
            var developmentUrl = "http://localhost:3000/budorhub";
            var productionUrl = "https://budorbeach.azurewebsites.net/budorhub";
            _connection = new HubConnectionBuilder()
                .WithUrl(environment.IsProduction() ? productionUrl : developmentUrl).WithAutomaticReconnect()
                .Build();
            _containerClient =
                new BlobContainerClient(config.GetConnectionString("AzureStorageConnectionString"), BlobContainerName);
            _camera = environment.IsProduction() ? MMALCamera.Instance : default;
        }

        public Task ConsoleLogMessage(string message)
        {
            _logger.LogInformation(message);
            return Task.CompletedTask;
        }

        public Task ReceiveCurrentSensorReading(SensorReadingModel model)
        {
            _logger.LogInformation(model.ToString());

            return Task.CompletedTask;
        }

        public async Task TakeImage(PiCameraSettings settings)
        {
            await TakePictureJobHelper.TakeImageAndUploadAsync(_environment, _camera, _logger, _containerClient,
                settings);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _connection.On<PiCameraSettings>(nameof(IBudorHubClient.TakeImage),
                async settings => { await TakeImage(settings); });
            await SignalRHelper.StartWithRetryAsync(_connection, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _connection.DisposeAsync();
        }
    }
}