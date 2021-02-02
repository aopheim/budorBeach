using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Interfaces;
using Shared.Models;
using Shared.PiCameraSettings;
using Shared.SignalR;

namespace rpiDaemon
{
    public class BudorHubPiClient : IBudorHubClient, IHostedService
    {
        private readonly HubConnection _connection;
        private readonly ILogger<BudorHubPiClient> _logger;

        public BudorHubPiClient(ILogger<BudorHubPiClient> logger)
        {
            _logger = logger;
            _connection = new HubConnectionBuilder().WithUrl("http://localhost:3000/budorhub").WithAutomaticReconnect()
                .Build();
        }

        public Task ConsoleLogMessage(string message)
        {
            _logger.LogInformation(message);
            return Task.CompletedTask;
        }

        public Task ReceiveCurrentSensorReading(SensorReadingModel model)
        {
            _logger.LogInformation(model.ToString());

            return Task.CompletedTask;
        }

        public Task TakeImage(PiCameraSettings settings)
        {
            _logger.LogInformation("Taking image from BudorHubPiClient");

            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _connection.On<PiCameraSettings>(nameof(IBudorHubClient.TakeImage), settings => { TakeImage(settings); });
            await SignalRHelper.ConnectWithRetryAsync(_connection, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _connection.DisposeAsync();
        }
    }
}