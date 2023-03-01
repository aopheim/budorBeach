using System.Threading;
using System.Threading.Tasks;
using Shared.PiCameraSettings;

namespace Services.Interfaces;

public interface IPictureService
{
    Task TakeImageAndUploadAsync(PiCameraSettings settings,CancellationToken cancellationToken);
}