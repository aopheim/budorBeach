using System.Threading.Tasks;
using rpiDaemon.Models;

namespace budorWeb.Interfaces
{
    public interface IBudorWebClient
    {
        Task ConsoleLogMessage(string message);
        Task ReceiveCurrentSensorReading(SensorReadingModel model);
    }
}