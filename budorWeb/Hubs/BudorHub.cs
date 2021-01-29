using System.Threading.Tasks;
using budorWeb.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;

namespace budorWeb.Hubs
{
    [UsedImplicitly]
    public class BudorHub : Hub<IBudorWebClient>
    {
        public async Task SendMessageToAllClients(string message)
        {
            await Clients.All.ConsoleLogMessage(message);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.ConsoleLogMessage("SignalR is connected!");

            await base.OnConnectedAsync();
        }
    }
}