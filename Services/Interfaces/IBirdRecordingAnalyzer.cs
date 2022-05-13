using System.Threading;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBirdRecordingAnalyzer
    {
        bool IsRunning();
        Task RunAnalyzer(CancellationToken cancellationToken);
    }
}