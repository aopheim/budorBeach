using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Innovative.SolarCalculator;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MMALSharp;
using Quartz;
using Shared.PiCameraSettings;

namespace rpiDaemon.Jobs
{
    [UsedImplicitly]
    public class TakePictureJob : IJob
    {
        private const string BlobContainerName = "images";

        private readonly MMALCamera _camera;
        private readonly IConfiguration _config;
        private readonly BlobContainerClient _containerClient;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<TakePictureJob> _logger;

        public TakePictureJob()
        {
        }

        public TakePictureJob(ILogger<TakePictureJob> logger, IConfiguration config, IWebHostEnvironment environment)
        {
            _logger = logger;
            _config = config;
            _environment = environment;
            _containerClient =
                new BlobContainerClient(_config.GetConnectionString("AzureStorageConnectionString"), BlobContainerName);

            _camera = _environment.IsProduction() ? MMALCamera.Instance : default;
            MMALCameraConfig.Debug = true;
        }


        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            var solarTimes = new SolarTimes(now, 60.974951, 11.285140);
            var sunrise = solarTimes.Sunrise;
            var sunset = solarTimes.Sunset;

            if (now > sunrise.AddHours(-1) && now < sunset.AddHours(1))
                await TakePictureJobHelper.TakeImageAndUploadAsync(_environment, _camera, _logger, _containerClient,
                    new PiCameraSettings());
        }
    }
}