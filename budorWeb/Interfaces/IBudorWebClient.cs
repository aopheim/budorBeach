using System.Threading.Tasks;

namespace budorWeb.Interfaces
{
    public interface IBudorWebClient
    {
        Task ConsoleLogMessage(string message);
    }
}