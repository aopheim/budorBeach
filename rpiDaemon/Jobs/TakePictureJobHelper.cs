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
using rpiDaemon.DateTimeHelpers;
using Shared.Azure;
using Shared.PiCameraSettings;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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
                var now = DateTime.UtcNow;
                var folderName = DateTimeParser.GetFolderName(now);
                var fileName = DateTimeParser.GetFileName(now);
                var folderPath = $"/home/pi/images/{folderName}";
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
                var compressedImagePath = await CompressJpgImage(fullPath);
                await UploadImageToContainerClient(thumbnailContainerClient, folderName, fileName, compressedImagePath);

                Directory.Delete(folderPath, true);
            }
        }

        private static async Task<string> CompressJpgImage(string fullInputPath)
        {
            var outputPath = fullInputPath.Replace(".jpg", "-resized.jpg");

            await using var input = File.OpenRead(fullInputPath);
            var image = await Image.LoadAsync(input);
            var newWidth = image.Width / 4;
            var newHeight = image.Height / 4;
            image.Mutate(img => img.Resize(newWidth, newHeight));
            await image.SaveAsync(outputPath);

            return outputPath;
        }

        private static async Task UploadImageToContainerClient(BlobContainerClient containerClient, string folderName,
            string fileName, string fullPath)
        {
            var blobClient = containerClient.GetBlobClient($"{folderName}/{fileName}.jpg");
            await using var uploadFileStream = File.OpenRead(fullPath);
            var cTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await blobClient.UploadAsync(uploadFileStream, true, cTokenSource.Token);
            uploadFileStream.Close();
            await AzureStorageHelper.SetBlobPropertiesAsync(blobClient);
        }
    }
}