using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using Shared.Interfaces;

namespace budorWeb.Pages
{
    public class ImagesModel : PageModel
    {
        private readonly IAzureStorageService _azureStorageService;
        private readonly IConfiguration _config;
        private readonly IRepositories _repos;

        public ImagesModel(IConfiguration config, IAzureStorageService azureStorageService, IRepositories repos)
        {
            _config = config;
            _azureStorageService = azureStorageService;
            _repos = repos;
        }

        public List<ImageDto> ImagesForDay { get; set; }
        public BlobContainerClient ThumbnailContainerClient { get; set; }
        public BlobContainerClient FullSizeImageContainerClient { get; set; }

        public void OnGet()
        {
            var today = DateTime.UtcNow.Date;
            ImagesForDay = _repos.ImageUploads.GetUploadsForDay(DateOnly.FromDateTime(today)).Select(iu =>
                    new ImageDto
                        { Name = iu.FileName, ImageUrl = iu.FullSizeImageUrl, ThumbnailUrl = iu.ThumbnailWebPImageUrl })
                .ToList();
        }
    }
}