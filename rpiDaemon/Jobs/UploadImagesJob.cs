using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.Interfaces;
using Shared;
using Shared.DateTimeHelpers;
using Shared.Interfaces;
using Shared.Models;

namespace rpiDaemon.Jobs;

[DisallowConcurrentExecution]
public class UploadImagesJob : IJob
{
    private const int MaxNumberToUpload = 10;
    private readonly IAzureStorageService _azureStorageService;
    private readonly IHostEnvironment _environment;
    private readonly IFileSystemService _fileSystemService;
    private readonly ILogger<UploadImagesJob> _logger;
    private readonly IPictureEditService _pictureEditService;
    private readonly IRepositories _repos;

    // WebP compression takes incredibly long time (~30 sec when running on Raspberry Pi. Seems like it is an issue when running on ARM64 architecture: https://github.com/SixLabors/ImageSharp/issues/2125 
    // Currently disabling it
    private readonly bool CompressToWebPFormat = false;

    public UploadImagesJob(IAzureStorageService azureStorageService, IFileSystemService fileSystemService,
        IPictureEditService pictureEditService, ILogger<UploadImagesJob> logger, IHostEnvironment environment,
        IRepositories repos)
    {
        _azureStorageService = azureStorageService;
        _fileSystemService = fileSystemService;
        _pictureEditService = pictureEditService;
        _logger = logger;
        _environment = environment;
        _repos = repos;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var imagesFolder = _environment.IsDevelopment()
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "budorBeach", "images")
            : GlobalConstants.ImagesFolder;
        var folderNamesForUpload = _fileSystemService.GetFolderNamesInFolder(imagesFolder).ToList();
        var imagesForUpload = folderNamesForUpload.SelectMany(folderName =>
        {
            var imagesInFolder =
                _fileSystemService.GetFileNamesWithoutExtensionInFolder(Path.Combine(imagesFolder, folderName));
            return imagesInFolder.Select(fileName => Path.Combine(folderName, fileName));
        }).ToList();

        if (!imagesForUpload.Any())
        {
            _logger.LogInformation("No images found in {folder}. Exiting.",
                imagesFolder);
            return;
        }

        _logger.LogInformation("Found {Count} files for upload. Uploading the first {Max}", imagesForUpload.Count(),
            MaxNumberToUpload);
        foreach (var filePath in imagesForUpload.Take(MaxNumberToUpload))
        {
            // Seeing weird behavior with Task being cancelled directly after starting upload. Trying to use another CancellationToken...
            var uploadTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            var linkedCToken =
                CancellationTokenSource.CreateLinkedTokenSource(uploadTimeout.Token, context.CancellationToken);
            var localFileLocation = Path.Combine(imagesFolder, filePath);
            await UploadImageToContainerClient(GlobalConstants.ImagesContainerName,
                $"{filePath}.jpg",
                $"{localFileLocation}.jpg", linkedCToken.Token);
            _logger.LogInformation($"Uploaded jpg image {localFileLocation}.jpg");
            string compressedImagePath = null;
            if (CompressToWebPFormat)
            {
                compressedImagePath =
                    await _pictureEditService.CompressJpgToWebPFormat($"{localFileLocation}.jpg", linkedCToken.Token);
                _logger.LogInformation($"Saved compressed image at {compressedImagePath}");
                await UploadImageToContainerClient(GlobalConstants.ThumbnailImagesContainerName,
                    $"{filePath}.webp", compressedImagePath, linkedCToken.Token);
            }

            await _repos.ImageUploads.AddAsync(new ImageUploadModel
            {
                FileName = $"{filePath}",
                TakenAtUtc = DateTimeParser.GetDateTimeFromFolderAndFileName(filePath) ?? DateTime.UtcNow,
                FullSizeImageUrl = _azureStorageService.GetBlobUrl(GlobalConstants.ImagesContainerName,
                    $"{filePath}.jpg"),
                ThumbnailJpgImageUrl = null,
                ThumbnailWebPImageUrl =
                    await _azureStorageService.ExistsAsync(GlobalConstants.ThumbnailImagesContainerName,
                        $"{filePath}.webp", linkedCToken.Token)
                        ? _azureStorageService.GetBlobUrl(GlobalConstants.ThumbnailImagesContainerName,
                            $"{filePath}.webp")
                        : null
            }, context.CancellationToken);
            _logger.LogInformation(
                "Saved ImageUpload entry {path} to ImageUploads repo. Deleting it and compressed image", filePath);

            _fileSystemService.DeleteFile($"{localFileLocation}.jpg");
            if (compressedImagePath != null)
                _fileSystemService.DeleteFile(compressedImagePath);
        }

        foreach (var folderName in folderNamesForUpload)
        {
            var folderPath = Path.Combine(imagesFolder, folderName);
            if (_fileSystemService.IsDirectoryEmpty(folderPath))
                _fileSystemService.DeleteDirectory(folderPath, false);
        }
    }

    private async Task UploadImageToContainerClient(string azureContainerName, string fileNameWithExtension,
        string fullPath, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Uploading image to Azure container {ContainerName}. Uploading from path {fullPath}, giving file name {fileName}",
            azureContainerName, fullPath, fileNameWithExtension);
        await _azureStorageService.UploadFileFromPath(azureContainerName, fullPath, fileNameWithExtension,
            cancellationToken);
        if (fileNameWithExtension.EndsWith(".jpg"))
        {
            var blobClient = _azureStorageService.GetBlobClient(azureContainerName, fileNameWithExtension);
            await _azureStorageService.SetJpgBlobPropertiesAsync(blobClient, cancellationToken);
        }
    }
}