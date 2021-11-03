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
        public static async Task<bool> StartWithRetryAsync(HubConnection connection,
            CancellationToken cancellationToken)
        {
            // Keep trying to until we can start or the token is canceled.
            while (true)
                try
                {
                    await connection.StartAsync(cancellationToken);
                    Debug.Assert(connection.State == HubConnectionState.Connected);
                    return true;
                }
                catch when (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    await Task.Delay(5000, cancellationToken);
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
            connection.Reconnecting += e =>
            {
                logger.LogError("Reconnecting connection...");
                logger.LogError(e, e.Message);
                return Task.CompletedTask;
            };
            connection.Reconnected += message =>
            {
                logger.LogInformation("Connection successfully reconnected");
                logger.LogInformation(message);
                return Task.CompletedTask;
            };
        }
    }
}