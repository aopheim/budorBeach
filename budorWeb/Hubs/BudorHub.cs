using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace budorWeb.Hubs
{
    public class BudorHub : Hub
    {
        public async Task SendMessageToAllClients(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}