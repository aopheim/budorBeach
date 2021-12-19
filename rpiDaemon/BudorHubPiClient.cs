using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using CameraService.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using rpiDaemon.Jobs;
using Shared;
using Shared.Azure;
using Shared.Interfaces;
using Shared.Models;
using Shared.PiCameraSettings;
using Shared.SignalR;

namespace rpiDaemon
{
    [UsedImplicitly]
    public class BudorHubPiClient : IBudorHubClient, IHostedService
    {
        private readonly ICameraService _cameraService;
        private readonly IWebHostEnvironment _environment;
        private readonly BlobContainerClient _imagesContainerClient;
        private readonly ILogger<BudorHubPiClient> _logger;
        private readonly ISignalRService _signalRService;
        private readonly BlobContainerClient _thumbnailsContainerClient;

        public BudorHubPiClient(ILogger<BudorHubPiClient> logger, IWebHostEnvironment environment,
            IConfiguration config,
            ICameraService cameraService, ISignalRService signalRService
        )
        {
            _logger = logger;
            _environment = environment;
            _cameraService = cameraService;
            _signalRService = signalRService;
            _imagesContainerClient =
                AzureStorageHelper.GetBlobContainerClient(config, GlobalConstants.ImagesContainerName);
            _thumbnailsContainerClient =
                AzureStorageHelper.GetBlobContainerClient(config, GlobalConstants.ThumbnailImagesContainerName);
            RegisterClientMethods();
        }

        public Task ConsoleLogMessage(string message, CancellationToken cancellationToken)
        {
            _logger.LogInformation(message);
            return Task.CompletedTask;
        }

        public Task SendSensorReading(SensorReadingModel model, CancellationToken cancellationToken)
        {
            _logger.LogInformation(model.ToString());

            return Task.CompletedTask;
        }

        public async Task TakeImage(PiCameraSettings settings, CancellationToken cancellationToken)
        {
            await TakePictureJobHelper.TakeImageAndUploadAsync(_environment, _cameraService, _logger,
                _imagesContainerClient,
                _thumbnailsContainerClient, settings);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _signalRService.StartWithRetryAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _signalRService.StopAsync(cancellationToken);
        }

        private void RegisterClientMethods()
        {
            var standardCToken = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
            _signalRService.RegisterClientMethod<PiCameraSettings>(nameof(ISignalRService.TakeImage),
                async settings => await TakeImage(settings, standardCToken));
            _signalRService.RegisterClientMethod<string>(nameof(ISignalRService.ConsoleLogMessage),
                message => ConsoleLogMessage(message, standardCToken));
        }
    }
}