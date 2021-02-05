using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Interfaces
{
    public interface IBudorHubClient
    {
        Task ConsoleLogMessage(string message);
        Task ReceiveCurrentSensorReading(SensorReadingModel model);
        Task TakeImage(PiCameraSettings.PiCameraSettings settings);
    }
}