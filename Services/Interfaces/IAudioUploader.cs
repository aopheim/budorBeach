using System.Threading;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAudioUploader
    {
        bool IsRunning();
        Task StartUpload(CancellationToken cancellationToken);
    }
}