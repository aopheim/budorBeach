using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Models;
using Shared.PiCameraSettings;
using Shared.SignalR;

namespace Services
{
    public class SignalRService : ISignalRService, IAsyncDisposable
    {
        private readonly HubConnection _connection;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<SignalRService> _logger;

        public SignalRService(IWebHostEnvironment environment, ILogger<SignalRService> logger)
        {
            _environment = environment;
            _logger = logger;
            _connection = GetHubConnection();
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }

        public async Task ConsoleLogMessage(string message, CancellationToken cancellationToken)
        {
            await StartWithRetryAsync(cancellationToken);
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync(nameof(BudorHub.SendMessageToAllClients),
                    message, cancellationToken);
        }

        public async Task SendSensorReading(SensorReadingModel model, CancellationToken cancellationToken)
        {
            await StartWithRetryAsync(cancellationToken);
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync(nameof(BudorHub.SendSensorReadingModelToWebClient), model,
                    cancellationToken);
        }

        public async Task TakeImage(PiCameraSettings settings, CancellationToken cancellationToken)
        {
            await StartWithRetryAsync(cancellationToken);
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync(nameof(BudorHub.TakeImage), settings,
                    cancellationToken);
        }

        public async Task StartWithRetryAsync(CancellationToken cancellationToken)
        {
            SetupEventsForDebuggingConnection(_connection);
            if (_connection.State == HubConnectionState.Disconnected)
                try
                {
                    await _connection.StartAsync(cancellationToken);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError("Connecting with SignalR threw HttpRequestException");
                }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.StopAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public void RegisterClientMethod<T>(string methodName, Action<T> action)
        {
            _connection.On(methodName, action);
        }

        private HubConnection GetHubConnection()
        {
            return new HubConnectionBuilder().WithUrl(_environment.IsDevelopment()
                    ? GlobalConstants.DevelopmentHubUrl
                    : GlobalConstants.ProductionHubUrl).WithAutomaticReconnect()
                .Build();
        }

        private void SetupEventsForDebuggingConnection(HubConnection connection)
        {
            connection.Reconnecting += e =>
            {
                _logger.LogError("Reconnecting connection...");
                // _logger.LogError(e, e.Message);
                return Task.CompletedTask;
            };
            connection.Reconnected += message =>
            {
                _logger.LogInformation("Connection successfully reconnected!");
                _logger.LogInformation(message);
                return Task.CompletedTask;
            };
        }
    }
}