using System.Threading;

namespace Services.Interfaces
{
    public interface IProximityService
    {
        double GetDistance(CancellationToken cancellationToken);
    }
}