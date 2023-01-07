using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using rpiDaemon.DateTimeHelpers;
using Services.Interfaces;
using Shared;

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

        public List<BlobItem> ImageBlobsForDay { get; set; }
        public BlobContainerClient ThumbnailContainerClient { get; set; }
        public BlobContainerClient FullSizeImageContainerClient { get; set; }

        public void OnGet()
        {
            var today = DateTime.UtcNow;
            var folderName = DateTimeParser.GetFolderName(today);

            var thumbnailContainerClient =
                _azureStorageService.GetBlobContainerClient(GlobalConstants.ThumbnailImagesContainerName);
            ImageBlobsForDay = thumbnailContainerClient.GetBlobs().Where(blob => blob.Name.Contains(folderName))
                .OrderBy(blob => DateTimeParser.GetDateTimeFromFolderAndFileName(blob.Name)).ToList();
            ThumbnailContainerClient = thumbnailContainerClient;
            FullSizeImageContainerClient =
                _azureStorageService.GetBlobContainerClient(GlobalConstants.ImagesContainerName);
        }

        public void OnGetImagesForDay(string selectedDateAsString)
        {
            var folderName = selectedDateAsString;
        }
    }
}