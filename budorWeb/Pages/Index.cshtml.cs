using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace budorWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly IConfiguration _config;
        private readonly ILogger<IndexModel> _logger;
        private readonly string BlobContainerName = "images";
        private string _newestImageUrl;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
        {
            _config = config;
            _logger = logger;
            _cloudBlobClient = CloudStorageAccount.Parse(_config.GetConnectionString("AzureStorageConnectionString"))
                .CreateCloudBlobClient();
        }

        public List<CloudBlockBlob> AllImagesInBlob { get; set; }

        public async Task OnGetAsync()
        {
            var blobContainer = _cloudBlobClient.GetContainerReference(BlobContainerName);
            BlobContinuationToken continuationToken = null;
            var allImages = new List<CloudBlockBlob>();

            do
            {
                var response = await blobContainer.ListBlobsSegmentedAsync(default, true, default,
                    default, continuationToken, default, default);
                continuationToken = response.ContinuationToken;
                allImages.AddRange(response.Results.Cast<CloudBlockBlob>());
            } while (continuationToken != null);

            AllImagesInBlob = allImages;
        }
    }
}