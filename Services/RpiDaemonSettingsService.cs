using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Services.Interfaces;
using Shared;
using Shared.RpiDaemonSettings;

namespace Services;

public class RpiDaemonSettingsService : IRpiDaemonSettingsService
{
    private readonly IAzureStorageService _azureStorageService;

    public RpiDaemonSettingsService(IAzureStorageService azureStorageService)
    {
        _azureStorageService = azureStorageService;
    }

    public async Task SetRpiDaemonSettings(IRpiDaemonSettings settings, CancellationToken cancellationToken)
    {
        var jsonString = JsonSerializer.Serialize(settings);
        var byteArray = Encoding.UTF8.GetBytes(jsonString);
        var stream = new MemoryStream(byteArray);
        await _azureStorageService.UploadFileFromStream(stream, GlobalConstants.RpiDaemonSettingsContainerName,
            GlobalConstants.RpiDaemonSettingsFileName, cancellationToken);
    }

    public async Task<IRpiDaemonSettings> GetRpiDaemonSettings(CancellationToken cancellationToken)
    {
        var blobClient = _azureStorageService.GetBlobClient(GlobalConstants.RpiDaemonSettingsContainerName,
            GlobalConstants.RpiDaemonSettingsFileName);
        var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream, cancellationToken);
        stream.Position = 0;
        var settingsAsString = await new StreamReader(stream).ReadToEndAsync();
        return JsonSerializer.Deserialize<RpiDaemonSettings>(settingsAsString);
    }
}