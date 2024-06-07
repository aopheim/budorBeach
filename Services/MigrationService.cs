using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BirdSpeciesNameTranslator;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;

namespace Services;

public class MigrationService : IMigrationService
{
    private const string JpgSuffix = ".jpg";
    private const string WebPSuffix = ".webp";
    private readonly IAzureStorageService _azureStorageService;
    private readonly IImageConverter _imageConverter;
    private readonly ILogger<MigrationService> _logger;
    private readonly IRepositories _repos;
    private readonly IBirdSpeciesNameTranslator _translator;

    public MigrationService(IAzureStorageService azureStorageService, ILogger<MigrationService> logger,
        IImageConverter imageConverter, IRepositories repos, IBirdSpeciesNameTranslator translator)
    {
        _azureStorageService = azureStorageService;
        _logger = logger;
        _imageConverter = imageConverter;
        _repos = repos;
        _translator = translator;
    }


    public async Task MigrateJpgImagesToWebP(CancellationToken cancellationToken)
    {
        var containerNamesToMigrate = new List<string>
            { GlobalConstants.ThumbnailImagesContainerName, GlobalConstants.ImagesContainerName };
        foreach (var containerName in containerNamesToMigrate)
        {
            _logger.LogInformation($"Starting migration for blob container {containerName}");
            var allBlobsInContainer = _azureStorageService.GetBlobContainerClient(containerName)
                .GetBlobsAsync(cancellationToken: cancellationToken);
            await foreach (var blob in allBlobsInContainer)
            {
                if (!blob.Name.EndsWith(JpgSuffix))
                    continue;
                if (await _azureStorageService.ExistsAsync(containerName, GetNewFileName(blob.Name),
                        cancellationToken))
                {
                    _logger.LogInformation($"Blob {blob.Name} already converted. Skipping.");
                    continue;
                }

                _logger.LogInformation($"Starting convert for {blob.Name}");
                var blobClient = _azureStorageService.GetBlobClient(containerName, blob.Name);
                var jpgStream = new MemoryStream();
                _logger.LogInformation($"Downloading blob {blob.Name}...");
                var response = await blobClient.DownloadToAsync(jpgStream, cancellationToken);
                if (response.IsError)
                {
                    _logger.LogError(
                        $"An error occurred when downloading {blob.Name}. HTTP status: {response.Status}. Skipping");
                    continue;
                }

                var webPStream = await _imageConverter.Jpg2WebP(jpgStream, cancellationToken);
                _logger.LogInformation(
                    $"Uploading WebP image with name {GetNewFileName(blob.Name)} to blob container {containerName}...");
                webPStream.Position = 0;
                await _azureStorageService.UploadFileFromStream(webPStream, containerName, GetNewFileName(blob.Name),
                    cancellationToken);
            }
        }

        _logger.LogInformation("Migration complete!");
    }

    public async Task MigrateSpeciesRecognitionsToIncludeEBirdTaxonomyId(CancellationToken cancellationToken)
    {
        var recognitionsToMigrate = (await _repos.SpeciesRecognitions.WhereAsync(recognition =>
            recognition.EBirdTaxonomyId == null && recognition.LatinName.Length > 0 &&
            recognition.EnglishName.Length > 0, cancellationToken)).ToList();
        _logger.LogInformation($"Found {recognitionsToMigrate.Count} recognitions to migrate");
        foreach (var recognition in recognitionsToMigrate)
        {
            recognition.EBirdTaxonomyId =
                _translator.GetTaxonomyCodeFromLatinAndEnglishName(recognition.LatinName, recognition.EnglishName);
            await _repos.SpeciesRecognitions.UpdateAsync(recognition, cancellationToken);
        }

        _logger.LogInformation("Migration of recognitions finished");
    }

    private string GetNewFileName(string existingFileName)
    {
        return existingFileName.Replace(JpgSuffix, WebPSuffix);
    }
}