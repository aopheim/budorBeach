using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBirdNetServer
    {
        Task<bool> StartBirdNetServer();
        Task<string> PostAsync(FileStream audioFileAsStream, CancellationToken cancellationToken);
    }
}