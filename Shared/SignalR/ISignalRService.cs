using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Interfaces;

namespace Shared.SignalR
{
    public interface ISignalRService : IBudorHubClient
    {
        public Task StartWithRetryAsync(CancellationToken cancellationToken);
        public Task StopAsync(CancellationToken cancellationToken);
        public void RegisterClientMethod<T>(string methodName, Action<T> action);
    }
}