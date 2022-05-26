using System.Threading;
using System.Threading.Tasks;

namespace AudioService.Interfaces
{
    public interface IAudioService
    {
        Task<bool> CaptureAudio(CancellationToken cancellationToken);
        bool IsRunning();
    }
}