using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Shared.Interfaces;
using Shared.Models;

namespace Shared
{
    [UsedImplicitly]
    public class BudorHub : Hub<IBudorHubClient>
    {
        private readonly ILogger<BudorHub> _logger;

        public BudorHub(ILogger<BudorHub> logger)
        {
            _logger = logger;
        }

        public async Task SendMessageToAllClients(string message)
        {
            var cTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await Clients.All.ConsoleLogMessage(message, cTokenSource.Token);
        }

        public async Task SendSensorReadingModelToWebClient(SensorReadingModel model)
        {
            var cTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await Clients.All.SendSensorReading(model, cTokenSource.Token);
        }

        public async Task TakeImage(PiCameraSettings.PiCameraSettings settings)
        {
            var cTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await Clients.All.TakeImage(settings, cTokenSource.Token);
        }
    }
}