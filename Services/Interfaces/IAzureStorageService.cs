using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Services.Interfaces
{
    public interface IAzureStorageService
    {
        BlobContainerClient GetBlobContainerClient(string containerName);
        BlobClient GetBlobClient(string containerName, string fileNameWithExtension);

        Task<bool> UploadFileFromPath(string containerName, string filePath, string fileNameWithExtension,
            CancellationToken cancellationToken);

        Task UploadFileFromStream(Stream inputStream, string containerName, string fileNameWithExtension,
            CancellationToken cancellationToken);

        Task<bool> ExistsAsync(string containerName, string fileNameWithExtension,
            CancellationToken cancellationToken);

        Task SetJpgBlobPropertiesAsync(BlobClient blob, CancellationToken cancellationToken);
        string GetBlobUrl(string containerName, string fileNameWithExtension);
    }
}