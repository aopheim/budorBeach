using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;

namespace Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _config;

        public AzureStorageService(IConfiguration config)
        {
            _config = config;
            _blobServiceClient = new BlobServiceClient(config[AzureStorageConnectionString]);
        }

        private static string AudioRecordingsContainerName => "audiorecordings";
        private static string AzureStorageConnectionString => "AzureStorageConnectionString";


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
            return _blobServiceClient.GetBlobContainerClient(containerName)
                .GetBlobClient(fileNameWithExtension);
        }
    }
}