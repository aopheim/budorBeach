using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Handlers;
using Quartz;

namespace rpiDaemon.Jobs
{
    [UsedImplicitly]
    public class TakePictureJob : IJob
    {
        private const string BlobContainerName = "images";
        private readonly IConfiguration _config;
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<TakePictureJob> _logger;

        public TakePictureJob()
        {
        }

        public TakePictureJob(ILogger<TakePictureJob> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _containerClient =
                new BlobContainerClient(_config.GetConnectionString("AzureStorageConnectionString"), BlobContainerName);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            var folderName = $"{now.Year}-{now.Month}-{now.Day}";
            var fileName = $"{now.Hour}-{now.Minute}-{now.Second}";
            var fullPath = $"/home/pi/images/{folderName}/{fileName}.jpg";
            try
            {
                var cam = MMALCamera.Instance;
                using (var imgCaptureHandler = new ImageStreamCaptureHandler(fullPath))
                {
                    await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
                }

                cam.Cleanup();
            }
            catch (Exception e)
            {
                if (e is DllNotFoundException) _logger.LogError("Raspberry Pi camera not found");
                else
                    throw;
            }

            var blobClient = _containerClient.GetBlobClient($"/{folderName}/{fileName}.jpg");
            await using var uploadFileStream = File.OpenRead(fullPath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
        }
    }
}