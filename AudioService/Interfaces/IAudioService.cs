using System.Threading;
using System.Threading.Tasks;

namespace AudioService.Interfaces
{
    public interface IAudioService
    {
        Task CaptureAudioContinuously(CancellationToken cancellationToken);
        bool IsRunning();
    }
}