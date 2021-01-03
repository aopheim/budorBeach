using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using rpiDaemon;
using rpiDaemon.Models;

namespace budorWeb.Pages
{
    public class BudorBeachModel : PageModel
    {
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BudorBeachModel> _logger;
        private readonly string BlobContainerName = "images";

        public BudorBeachModel(ILogger<BudorBeachModel> logger, IConfiguration config, ApplicationDbContext context)
        {
            _context = context;
            _config = config;
            _logger = logger;
            _cloudBlobClient = CloudStorageAccount.Parse(_config.GetConnectionString("AzureStorageConnectionString"))
                .CreateCloudBlobClient();
        }

        public List<CloudBlockBlob> AllImagesInBlob { get; set; }
        public List<SensorReadingModel> AllSensorReadings { get; set; }
        public List<SensorReadingModel> SensorReadingsFromLastSevenDays { get; set; }
        public List<SensorReadingModel> SensorReadingsFromLastMonth { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var sevenDaysAgo = now.AddDays(-7);
            var oneMonthAgo = now.AddMonths(-1);
            AllSensorReadings = await _context.SensorReadings.Where(model => true).ToListAsync(cancellationToken);
            SensorReadingsFromLastSevenDays =
                await _context.SensorReadings.Where(model => model.MeasuredAtUtc > sevenDaysAgo)
                    .ToListAsync(cancellationToken);
            SensorReadingsFromLastMonth = await _context.SensorReadings
                .Where(model => model.MeasuredAtUtc > oneMonthAgo).ToListAsync(cancellationToken);

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