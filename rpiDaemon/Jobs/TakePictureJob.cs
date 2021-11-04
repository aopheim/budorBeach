using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using CameraService.Interfaces;
using Innovative.SolarCalculator;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMALSharp;
using Quartz;
using Shared;
using Shared.Azure;
using Shared.PiCameraSettings;

namespace rpiDaemon.Jobs
{
    [UsedImplicitly]
    public class TakePictureJob : IJob
    {
        private readonly ICameraService _cameraService;
        private readonly BlobContainerClient _containerClient;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<TakePictureJob> _logger;
        private readonly BlobContainerClient _thumbnailsContainerClient;

        public TakePictureJob(ILogger<TakePictureJob> logger, IConfiguration config, IWebHostEnvironment environment,
            ICameraService cameraService)
        {
            _logger = logger;
            _environment = environment;
            _cameraService = cameraService;
            _containerClient = AzureStorageHelper.GetBlobContainerClient(config, GlobalConstants.ImagesContainerName);
            _thumbnailsContainerClient =
                AzureStorageHelper.GetBlobContainerClient(config, GlobalConstants.ThumbnailImagesContainerName);

            MMALCameraConfig.Debug = true;
        }


        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            var solarTimes = new SolarTimes(now, 60.974951, 11.285140);
            var sunrise = solarTimes.Sunrise;
            var sunset = solarTimes.Sunset;

            if (now > sunrise && now < sunset)
                await TakePictureJobHelper.TakeImageAndUploadAsync(_environment, _cameraService, _logger,
                    _containerClient,
                    _thumbnailsContainerClient,
                    new PiCameraSettings());
        }
    }
}