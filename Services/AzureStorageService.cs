using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Services.Interfaces;

namespace Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _config;

        public AzureStorageService(IConfiguration config, IWebHostEnvironment environment)
        {
            _config = config;
            _blobServiceClient =
                new BlobServiceClient(environment.IsDevelopment()
                    ? config[AzuriteStorageConnectionString]
                    : config[AzureStorageConnectionString]);
        }

        private static string AzureStorageConnectionString => "AzureStorageConnectionString";
        private static string AzuriteStorageConnectionString => "AzuriteStorageConnectionString";


        public async Task<bool> UploadFileFromPath(string containerName, string filePath, string fileNameWithExtension,
            CancellationToken cancellationToken)
        {
            var blobClient = GetBlobClient(containerName, fileNameWithExtension);
            await using var uploadFileStream = File.OpenRead(filePath);
            await blobClient.UploadAsync(uploadFileStream, true, cancellationToken);
            uploadFileStream.Close();

            return true;
        }

        public async Task<bool> ExistsAsync(string containerName, string fileNameWithExtension,
            CancellationToken cancellationToken)
        {
            var blobClient = GetBlobClient(containerName, fileNameWithExtension);

            return await blobClient.ExistsAsync(cancellationToken);
        }

        private BlobClient GetBlobClient(string containerName, string fileNameWithExtension)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            containerClient.CreateIfNotExists();

            return containerClient.GetBlobClient(fileNameWithExtension);
        }
    }
}