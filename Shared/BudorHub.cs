using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Shared.Interfaces;
using Shared.Models;

namespace Shared
{
    [UsedImplicitly]
    public class BudorHub : Hub<IBudorHubClient>
    {
        public async Task SendMessageToAllClients(string message)
        {
            await Clients.All.ConsoleLogMessage(message);
        }

        public async Task SendSensorReadingModelToWebClient(SensorReadingModel model)
        {
            await Clients.All.ReceiveCurrentSensorReading(model);
        }

        public async Task TakeImage(PiCameraSettings.PiCameraSettings settings)
        {
            await Clients.All.TakeImage(settings);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}