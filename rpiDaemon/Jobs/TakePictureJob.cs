using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Config;
using MMALSharp.Handlers;
using Quartz;
using rpiDaemon.DateTimeHelpers;
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
            if (_environment.IsDevelopment())
            {
                _logger.LogInformation($"Mocked image taken at {DateTime.UtcNow}");
            }
            else
            {
                var now = DateTime.UtcNow;
                var folderName = DateTimeParser.GetFolderName(now);
                var fileName = DateTimeParser.GetFileName(now);
                var fullPath = $"/home/pi/images/{folderName}/{fileName}.jpg";
                try
                {
                    using var imgCaptureHandler = new ImageStreamCaptureHandler(fullPath);

                    MMALCameraConfig.ISO = PiCameraSettings.Iso;
                    MMALCameraConfig.ShutterSpeed = PiCameraSettings.ShutterTime;
                    MMALCameraConfig.Annotate = new AnnotateImage("Budor Beach", 15, Color.DarkGray);

                    _camera.ConfigureCameraSettings();

                    await _camera.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
                }
                catch (Exception e)
                {
                    if (e is DllNotFoundException) _logger.LogError("Raspberry Pi camera not found");
                    else
                        throw;
                }

                _logger.LogInformation($"Picture taken at {DateTime.UtcNow}");
                var blobClient = _containerClient.GetBlobClient($"{folderName}/{fileName}.jpg");
                await using var uploadFileStream = File.OpenRead(fullPath);
                await blobClient.UploadAsync(uploadFileStream, true);
                uploadFileStream.Close();
            }
        }
    }
}