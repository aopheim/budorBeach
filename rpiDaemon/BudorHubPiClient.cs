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
using Shared;
using Shared.Azure;
using Shared.Interfaces;
using Shared.Models;
using Shared.PiCameraSettings;
using Shared.SignalR;

namespace rpiDaemon
{
    public class BudorHubPiClient : IBudorHubClient, IHostedService
    {
        private readonly MMALCamera _camera;

        private readonly HubConnection _connection;
        private readonly IWebHostEnvironment _environment;
        private readonly BlobContainerClient _imagesContainerClient;
        private readonly ILogger<BudorHubPiClient> _logger;
        private readonly BlobContainerClient _thumbnailsContainerClient;

        public BudorHubPiClient(ILogger<BudorHubPiClient> logger, IWebHostEnvironment environment,
            IConfiguration config)
        {
            _logger = logger;
            _environment = environment;
            _connection = new HubConnectionBuilder()
                .WithUrl(environment.IsProduction()
                    ? GlobalConstants.ProductionHubUrl
                    : GlobalConstants.DevelopmentHubUrl).WithAutomaticReconnect()
                .Build();
            _imagesContainerClient =
                AzureStorageHelper.GetBlobContainerClient(config, GlobalConstants.ImagesContainerName);
            _thumbnailsContainerClient =
                AzureStorageHelper.GetBlobContainerClient(config, GlobalConstants.ThumbnailImagesContainerName);
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
            await TakePictureJobHelper.TakeImageAndUploadAsync(_environment, _camera, _logger, _imagesContainerClient,
                _thumbnailsContainerClient, settings);
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