using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using CameraService.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Azure;
using Shared.DateTimeHelpers;
using Shared.PiCameraSettings;
using SixLabors.ImageSharp;

namespace rpiDaemon.Jobs
{
    public static class TakePictureJobHelper
    {
        public static async Task TakeImageAndUploadAsync(IWebHostEnvironment environment, ICameraService cameraService,
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
                if (cameraService.CameraIsInUse())
                {
                    logger.LogInformation("Camera is in use. Skipping taking image");
                    return;
                }

                var now = DateTime.UtcNow;
                var folderName = DateTimeParser.GetFolderName(now);
                var fileName = DateTimeParser.GetFileName(now);
                var folderPath = $"{GlobalConstants.ImagesFolder}{folderName}";
                var fullPath = folderPath + $"/{fileName}.jpg";

                try
                {
                    await cameraService.TakeImage(fullPath, settings);
                }
                catch (Exception e)
                {
                    if (e is DllNotFoundException) logger.LogError("Raspberry Pi camera not found");
                    else
                        throw;
                }

                logger.LogInformation(
                    $"Picture taken at {DateTime.UtcNow}. Camera settings: {JsonSerializer.Serialize(settings)}");
                await UploadImageToContainerClient(containerClient, folderName, fileName, fullPath);
                var compressedImagePath = await CompressToWebPFormat(fullPath);
                await UploadImageToContainerClient(thumbnailContainerClient, folderName, fileName, compressedImagePath);

                Directory.Delete(folderPath, true);
            }
        }

        private static async Task<string> CompressToWebPFormat(string fullInputPath)
        {
            var outputPath = fullInputPath.Replace(".jpg", ".webp");

            await using var input = File.OpenRead(fullInputPath);
            var image = await Image.LoadAsync(input);
            await image.SaveAsWebpAsync(outputPath);

            return outputPath;
        }

        private static async Task UploadImageToContainerClient(BlobContainerClient containerClient, string folderName,
            string fileName, string fullPath)
        {
            var blobClient = containerClient.GetBlobClient($"{folderName}/{fileName}.jpg");
            await using var uploadFileStream = File.OpenRead(fullPath);
            var cTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await blobClient.UploadAsync(uploadFileStream, true, cTokenSource.Token);
            uploadFileStream.Close();
            await AzureStorageHelper.SetJpgBlobPropertiesAsync(blobClient);
        }
    }
}