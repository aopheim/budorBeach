using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using Shared;
using Shared.Azure;
using Shared.DateTimeHelpers;

namespace budorWeb.Pages
{
    public class ImagesModel : PageModel
    {
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConfiguration _config;

        public ImagesModel(IConfiguration config, IAzureStorageService azureStorageService)
        {
            _config = config;
            _azureStorageService = azureStorageService;
        }

        public List<ImageDto> ImagesForDay { get; set; }
        public BlobContainerClient ThumbnailContainerClient { get; set; }
        public BlobContainerClient FullSizeImageContainerClient { get; set; }

        public void OnGet()
        {
            var today = DateTime.UtcNow;
            var folderName = DateTimeParser.GetFolderName(today);

            ThumbnailContainerClient =
                _azureStorageService.GetBlobContainerClient(GlobalConstants.ThumbnailImagesContainerName);
            FullSizeImageContainerClient =
                _azureStorageService.GetBlobContainerClient(GlobalConstants.ImagesContainerName);

            ImagesForDay = ThumbnailContainerClient.GetBlobs().Where(blob => blob.Name.Contains(folderName))
                .OrderBy(blob => DateTimeParser.GetDateTimeFromFolderAndFileName(blob.Name)).Select(b => new ImageDto
                {
                    Name = b.Name,
                    ImageUrl = b.GetUrlForBlob(FullSizeImageContainerClient, ".jpg"),
                    ThumbnailUrl = b.GetUrlForBlob(ThumbnailContainerClient, ".webp")
                }).ToList();
        }
    }
}