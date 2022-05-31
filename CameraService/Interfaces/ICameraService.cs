using System.Threading;
using System.Threading.Tasks;
using Shared.PiCameraSettings;

namespace CameraService.Interfaces
{
    public interface ICameraService
    {
        bool CameraIsInUse();
        Task TakeImage(string fullPath, PiCameraSettings settings);
        Task CaptureVideo(string fullPath, int secondsToRecord);
        Task StartVideoSurveillance(string fullPath, int secondsToRecord, CancellationToken cancellationToken);
        Task StartVideoStream(CancellationToken cancellationToken);
    }
}