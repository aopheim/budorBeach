using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CameraService.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared;
using Shared.DateTimeHelpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.PiCameraSettings;

namespace Services;

public class PictureService : IPictureService
{
    private readonly IAzureStorageService _azureStorageService;
    private readonly ICameraService _cameraService;
    private readonly IWebHostEnvironment _environment;
    private readonly IFileSystemService _fileSystemService;
    private readonly ILogger<PictureService> _logger;
    private readonly IPictureEditService _pictureEditService;
    private readonly IRepositories _repos;

    public PictureService(IWebHostEnvironment environment, ILogger<PictureService> logger, ICameraService cameraService,
        IAzureStorageService azureStorageService, IFileSystemService fileSystemService,
        IPictureEditService pictureEditService, IRepositories repos)
    {
        _environment = environment;
        _logger = logger;
        _cameraService = cameraService;
        _azureStorageService = azureStorageService;
        _fileSystemService = fileSystemService;
        _pictureEditService = pictureEditService;
        _repos = repos;
    }

    public async Task TakeImageAndUploadAsync(PiCameraSettings settings, CancellationToken cancellationToken)
    {
        if (_environment.IsDevelopment())
        {
            _logger.LogInformation(
                $"Mocked image taken at {DateTime.UtcNow}. Camera settings: {JsonSerializer.Serialize(settings)}");
        }
        else
        {
            if (_cameraService.CameraIsInUse())
            {
                _logger.LogInformation("Camera is in use. Skipping taking image");
                return;
            }

            var now = DateTime.UtcNow;
            var folderName = DateTimeParser.GetFolderName(now);
            var fileName = DateTimeParser.GetFileName(now);
            var folderPath = $"{GlobalConstants.ImagesFolder}{folderName}";
            var fullPath = folderPath + $"/{fileName}.jpg";

            try
            {
                await _cameraService.TakeImage(fullPath, settings);
            }
            catch (Exception e)
            {
                if (e is DllNotFoundException) _logger.LogError("Raspberry Pi camera not found");
                else
                    throw;
            }

            _logger.LogInformation(
                $"Picture taken at {DateTime.UtcNow}. Camera settings: {JsonSerializer.Serialize(settings)}");
            await UploadImageToContainerClient(GlobalConstants.ImagesContainerName, $"{folderName}/{fileName}.jpg",
                fullPath, cancellationToken);
            var compressedImagePath = await _pictureEditService.CompressJpgToWebPFormat(fullPath);
            await UploadImageToContainerClient(GlobalConstants.ThumbnailImagesContainerName,
                $"{folderName}/{fileName}.webp", compressedImagePath, cancellationToken);
            await _repos.ImageUploads.AddAsync(new ImageUploadModel
            {
                FileName = $"{folderName}/{fileName}", TakenAtUtc = now,
                FullSizeImageUrl = _azureStorageService.GetBlobUrl(GlobalConstants.ImagesContainerName,
                    $"{folderName}/{fileName}.jpg"),
                ThumbnailJpgImageUrl = null,
                ThumbnailWebPImageUrl = _azureStorageService.GetBlobUrl(GlobalConstants.ThumbnailImagesContainerName,
                    $"{folderName}/{fileName}.webp")
            }, cancellationToken);
            await _repos.SaveChangesAsync(cancellationToken);

            _fileSystemService.DeleteDirectory(folderPath, true);
        }
    }

    private async Task UploadImageToContainerClient(string azureContainerName, string fileNameWithExtension,
        string fullPath, CancellationToken cancellationToken)
    {
        await _azureStorageService.UploadFileFromPath(azureContainerName, fullPath, fileNameWithExtension,
            cancellationToken);
        if (fileNameWithExtension.EndsWith(".jpg"))
        {
            var blobClient = _azureStorageService.GetBlobClient(azureContainerName, fileNameWithExtension);
            await _azureStorageService.SetJpgBlobPropertiesAsync(blobClient, cancellationToken);
        }
    }
}