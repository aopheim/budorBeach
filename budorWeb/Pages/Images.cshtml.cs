using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using rpiDaemon.DateTimeHelpers;
using Shared;
using Shared.Azure;

namespace budorWeb.Pages
{
    public class ImagesModel : PageModel
    {
        private readonly IConfiguration _config;

        public ImagesModel(IConfiguration config)
        {
            _config = config;
        }

        public List<BlobItem> ImageBlobsForDay { get; set; }
        public BlobContainerClient ThumbnailContainerClient { get; set; }
        public BlobContainerClient FullSizeImageContainerClient { get; set; }

        public void OnGet()
        {
            var today = DateTime.UtcNow;
            var folderName = DateTimeParser.GetFolderName(today);

            var thumbnailContainerClient =
                AzureStorageHelper.GetBlobContainerClient(_config, GlobalConstants.ThumbnailImagesContainerName);
            ImageBlobsForDay = thumbnailContainerClient.GetBlobs().Where(blob => blob.Name.Contains(folderName))
                .OrderBy(blob => DateTimeParser.GetDateTimeFromFolderAndFileName(blob.Name)).ToList();
            ThumbnailContainerClient = thumbnailContainerClient;
            FullSizeImageContainerClient =
                AzureStorageHelper.GetBlobContainerClient(_config, GlobalConstants.ImagesContainerName);
        }

        public void OnGetImagesForDay(string selectedDateAsString)
        {
            var folderName = selectedDateAsString;
        }
    }
}