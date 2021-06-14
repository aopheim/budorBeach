using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quartz;
using Shared;

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
                .WithUrl(GlobalConstants.DevelopmentHubUrl)
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
            await ConnectWithRetryAsync(_connection, context.CancellationToken);
            await _connection.InvokeAsync("SendMessageToAllClients", "Hello from Pi!",
                context.CancellationToken);
        }

        private async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token)
        {
            // Keep trying to until we can start or the token is canceled.
            while (true)
                try
                {
                    await connection.StartAsync(token);
                    Debug.Assert(connection.State == HubConnectionState.Connected);
                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    return false;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    await Task.Delay(5000, token);
                }
        }
    }
}