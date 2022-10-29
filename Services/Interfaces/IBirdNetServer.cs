using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Services.Interfaces
{
    public interface IBirdNetServer 
    {
        Task<string> PostAsync(string filePathToAudioFile, CancellationToken cancellationToken);
    }
}