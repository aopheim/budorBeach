using System.Threading;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Interfaces
{
    public interface IBudorHubClient
    {
        Task ConsoleLogMessage(string message,  CancellationToken cancellationToken);
        Task SendSensorReading(SensorReadingModel model, CancellationToken cancellationToken);
        Task TakeImage(PiCameraSettings.PiCameraSettings settings,  CancellationToken cancellationToken);
    }
}