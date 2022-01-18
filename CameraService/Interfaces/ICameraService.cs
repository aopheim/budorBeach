using System.Threading.Tasks;
using Shared.PiCameraSettings;

namespace CameraService.Interfaces
{
    public interface ICameraService
    {
        Task TakeImage(string fullPath, PiCameraSettings settings);
        Task CaptureVideo(string fullPath, int secondsToRecord);
    }
}