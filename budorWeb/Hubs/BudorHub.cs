using System.Threading.Tasks;
using budorWeb.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using rpiDaemon.Models;

namespace budorWeb.Hubs
{
    [UsedImplicitly]
    public class BudorHub : Hub<IBudorWebClient>
    {
        public async Task SendMessageToAllClients(string message)
        {
            await Clients.All.ConsoleLogMessage(message);
        }

        public async Task SendSensorReadingModelToWebClient(SensorReadingModel model)
        {
            await Clients.All.ReceiveCurrentSensorReading(model);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.ConsoleLogMessage("SignalR is connected!");

            await base.OnConnectedAsync();
        }
    }
}