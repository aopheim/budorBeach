using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quartz;

namespace rpiDaemon.Jobs
{
    public class TestSignalRJob : IJob
    {
        private readonly HubConnection _connection;
        private readonly ILogger<TestSignalRJob> _logger;

        public TestSignalRJob(ILogger<TestSignalRJob> logger)
        {
            _logger = logger;
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:3000/budorhub")
                .WithAutomaticReconnect()
                .Build();

            _connection.Closed += async e =>
            {
                _logger.LogError(e, e.Message);
                await Task.Delay(200);
                await _connection.StartAsync();
            };
            _connection.Reconnecting += e =>
            {
                _logger.LogError(e, e.Message);
                Debug.Assert(_connection.State == HubConnectionState.Reconnecting);
                return Task.CompletedTask;
            };
            _connection.Reconnected += message =>
            {
                _logger.LogInformation(message);
                Debug.Assert(_connection.State == HubConnectionState.Connected);
                return Task.CompletedTask;
            };
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Connecting to hub...");
            await ConnectWithRetryAsync(_connection, context.CancellationToken);
            _logger.LogInformation("Sending message...");
            await _connection.InvokeAsync("SendMessageToAllClients", "Hello from Pi!",
                context.CancellationToken);
            _logger.LogInformation("Message sent");
        }

        private async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token)
        {
            // Keep trying to until we can start or the token is canceled.
            while (true)
                try
                {
                    await connection.StartAsync(token);
                    Debug.Assert(connection.State == HubConnectionState.Connected);
                    _logger.LogInformation("Connection started");
                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    _logger.LogInformation("Cancellation token received");
                    return false;
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "Retrying to restart...");
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    await Task.Delay(5000, token);
                }
        }
    }
}