using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Config;
using MMALSharp.Handlers;
using rpiDaemon.DateTimeHelpers;
using Shared.Azure;
using Shared.PiCameraSettings;

namespace rpiDaemon.Jobs
{
    public static class TakePictureJobHelper
    {
        public static async Task TakeImageAndUploadAsync(IWebHostEnvironment environment, MMALCamera camera,
            ILogger logger, BlobContainerClient containerClient, BlobContainerClient thumbnailContainerClient,
            PiCameraSettings settings)
        {
            if (environment.IsDevelopment())
            {
                logger.LogInformation(
                    $"Mocked image taken at {DateTime.UtcNow}. Camera settings: {JsonSerializer.Serialize(settings)}");
            }
            else
            {
                var now = DateTime.UtcNow;
                var folderName = DateTimeParser.GetFolderName(now);
                var fileName = DateTimeParser.GetFileName(now);
                var folderPath = $"/home/pi/images/{folderName}";
                var fullPath = folderPath + $"/{fileName}.jpg";
                try
                {
                    using var imgCaptureHandler = new ImageStreamCaptureHandler(fullPath);

                    MMALCameraConfig.ISO = settings.Iso;
                    MMALCameraConfig.ShutterSpeed = settings.ShutterTime;
                    MMALCameraConfig.Annotate = new AnnotateImage("Budor Beach", 15, Color.DarkGray);

                    camera.ConfigureCameraSettings();

                    await camera.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
                }
                catch (Exception e)
                {
                    if (e is DllNotFoundException) logger.LogError("Raspberry Pi camera not found");
                    else
                        throw;
                }

                logger.LogInformation(
                    $"Picture taken at {DateTime.UtcNow}. Camera settings: {JsonSerializer.Serialize(settings)}");
                var blobClient = containerClient.GetBlobClient($"{folderName}/{fileName}.jpg");
                await using var uploadFileStream = File.OpenRead(fullPath);
                await blobClient.UploadAsync(uploadFileStream, true);
                uploadFileStream.Close();
                await AzureStorageHelper.SetBlobPropertiesAsync(blobClient);

                //using var compressedImage = new MagickImage(fullPath);
                //compressedImage.Resize(new Percentage(30));
                //compressedImage.Strip();
                //compressedImage.Write(fullPath);

                //blobClient = thumbnailContainerClient.GetBlobClient($"{folderName}/{fileName}.jpg");
                //await using var uploadThumbnailFileStream = File.OpenRead(fullPath);
                //await blobClient.UploadAsync(uploadFileStream, true);
                //uploadFileStream.Close();
                //await AzureStorageHelper.SetBlobPropertiesAsync(blobClient);

                Directory.Delete(folderPath, true);
            }
        }
    }
}