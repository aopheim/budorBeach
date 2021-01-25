using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace budorWeb.Hubs
{
    public class BudorHub : Hub
    {
        public async Task SendMessageToAllClients(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        //public async Task GetUtcNow()
        //{
        //    return await Task.FromResult(DateTime.UtcNow);
        //}
    }
}