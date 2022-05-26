using System.Threading;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAzureStorageService
    {
        Task<bool> UploadFileFromPath(string containerName, string filePath, string fileNameWithExtension,
            CancellationToken cancellationToken);

        Task<bool> ExistsAsync(string containerName, string fileNameWithExtension,
            CancellationToken cancellationToken);
    }
}