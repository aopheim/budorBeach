using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Shared.SignalR
{
    public static class SignalRHelper
    {
        public static async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token)
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
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    await Task.Delay(5000, token);
                }
        }

        public static HubConnection GetHubConnection(string hubUrl)
        {
            return new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();
        }

        public static void SetupEventsForDebuggingConnection(ILogger logger, HubConnection connection)
        {
            connection.Closed += async e =>
            {
                logger.LogError(e, e.Message);
                await Task.Delay(200);
                await connection.StartAsync();
            };
            connection.Reconnecting += e =>
            {
                logger.LogError(e, e.Message);
                Debug.Assert(connection.State == HubConnectionState.Reconnecting);
                return Task.CompletedTask;
            };
            connection.Reconnected += message =>
            {
                logger.LogInformation(message);
                Debug.Assert(connection.State == HubConnectionState.Connected);
                return Task.CompletedTask;
            };
        }
    }
}