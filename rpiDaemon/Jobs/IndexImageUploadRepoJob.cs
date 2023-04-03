using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.Interfaces;
using Shared;
using Shared.Azure;
using Shared.DateTimeHelpers;
using Shared.Interfaces;
using Shared.Models;

namespace rpiDaemon.Jobs;

public class IndexImageUploadRepoJob : IJob
{
    private readonly IAzureStorageService _azureStorageService;
    private readonly IRepository<ImageUploadModel> _imageUploadRepo;
    private readonly TimeSpan _indexingTimeOut = TimeSpan.FromMinutes(15);
    private readonly ILogger<IndexImageUploadRepoJob> _logger;
    private readonly IRepositories _repos;

    public IndexImageUploadRepoJob(IAzureStorageService azureStorageService,
        IRepositories repos, ILogger<IndexImageUploadRepoJob> logger)
    {
        _azureStorageService = azureStorageService;
        _imageUploadRepo = repos.ImageUploads;
        _repos = repos;
        _logger = logger;
    }


    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting indexing of image upload repo");

        var cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken,
            new CancellationTokenSource(_indexingTimeOut).Token).Token;
        if (!(await _imageUploadRepo.GetAllAsync(context.CancellationToken)).Any())
            await InitImageUploadRepo(cancellationToken);
        else
            await ReIndexImageUploadRepo(cancellationToken);
    }


    // First time indexing is run - fill up db with all images 
    private async Task InitImageUploadRepo(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ImageUploads db have no entries - adding entries based on Azure Storage");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var fullSizeImagesContainer =
            _azureStorageService.GetBlobContainerClient(GlobalConstants.ImagesContainerName);
        foreach (var fullSizeImage in fullSizeImagesContainer.GetBlobs().Where(b => b.Name.EndsWith(".jpg")))
        {
            var thumbnailJpgImage = _azureStorageService.GetBlobClient(GlobalConstants.ThumbnailImagesContainerName,
                fullSizeImage.Name);
            var thumbnailWebPImage = _azureStorageService.GetBlobClient(
                GlobalConstants.ThumbnailImagesContainerName, fullSizeImage.Name.Replace(".jpg", ".webp"));
            var fileNameWithoutExtension = fullSizeImage.Name.Replace(".jpg", "");
            var takenAtUtc = DateTimeParser.GetDateTimeFromFolderAndFileName(fileNameWithoutExtension);
            if (takenAtUtc == null)
            {
                _logger.LogWarning($"Could not parse file name {fileNameWithoutExtension} to DateTime. Skipping.");
                continue;
            }

            await _imageUploadRepo.AddAsync(new ImageUploadModel
            {
                FileName = fileNameWithoutExtension,
                TakenAtUtc = takenAtUtc.Value,
                FullSizeImageUrl = fullSizeImage.GetUrlForBlob(fullSizeImagesContainer, ".jpg"),
                ThumbnailJpgImageUrl = await thumbnailJpgImage.ExistsAsync(cancellationToken)
                    ? thumbnailJpgImage.Uri.ToString()
                    : null,
                ThumbnailWebPImageUrl = await thumbnailWebPImage.ExistsAsync(cancellationToken)
                    ? thumbnailWebPImage.Uri.ToString()
                    : null
            }, cancellationToken);
            _logger.LogInformation($"Added ImageUpload entry for filename {fileNameWithoutExtension}");
            await _repos.SaveChangesAsync(cancellationToken);
        }

        stopwatch.Stop();
        _logger.LogInformation($"Image upload repo indexed in {stopwatch.Elapsed.TotalSeconds} s");
    }

    // Updating index - check that all images in the repo are actually in Azure Storage
    private async Task ReIndexImageUploadRepo(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Re-indexing ImageUpload db.");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (var imageIndex in await _imageUploadRepo.GetAllAsync(cancellationToken))
        {
            var fullSizeImageExist = await _azureStorageService.ExistsAsync(GlobalConstants.ImagesContainerName,
                imageIndex.FileName + ".jpg", cancellationToken);
            var webPImageExist = await _azureStorageService.ExistsAsync(
                GlobalConstants.ThumbnailImagesContainerName,
                imageIndex.FileName + ".webp", cancellationToken);
            var jpgThumbnailExist = await _azureStorageService.ExistsAsync(
                GlobalConstants.ThumbnailImagesContainerName,
                imageIndex.FileName + ".jpg", cancellationToken);

            if (!fullSizeImageExist && !webPImageExist && !jpgThumbnailExist)
            {
                _logger.LogInformation(
                    $"Found no images in Azure Storage for {imageIndex.FileName}. Deleting entry");
                await _imageUploadRepo.RemoveAsync(imageIndex, cancellationToken);
                await _repos.SaveChangesAsync(cancellationToken);
                return;
            }

            imageIndex.FullSizeImageUrl = fullSizeImageExist
                ? _azureStorageService.GetBlobUrl(GlobalConstants.ImagesContainerName,
                    imageIndex.FileName + ".jpg")
                : null;
            imageIndex.ThumbnailWebPImageUrl = webPImageExist
                ? _azureStorageService.GetBlobUrl(GlobalConstants.ThumbnailImagesContainerName,
                    imageIndex.FileName + ".webp")
                : null;
            imageIndex.ThumbnailJpgImageUrl = jpgThumbnailExist
                ? _azureStorageService.GetBlobUrl(GlobalConstants.ThumbnailImagesContainerName,
                    imageIndex.FileName + ".jpg")
                : null;

            await _repos.SaveChangesAsync(cancellationToken);
        }

        stopwatch.Stop();
        _logger.LogInformation($"Re-indexing ran in {stopwatch.Elapsed.TotalSeconds} s");
    }
}