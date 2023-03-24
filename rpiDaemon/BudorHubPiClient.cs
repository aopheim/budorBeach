using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared.Interfaces;
using Shared.Models;
using Shared.PiCameraSettings;
using Shared.SignalR;
using SimpleInjector;

namespace rpiDaemon
{
    [UsedImplicitly]
    public class BudorHubPiClient : IBudorHubClient, IHostedService
    {
        private readonly Container _container;
        private readonly ILogger<BudorHubPiClient> _logger;
        private readonly ISignalRService _signalRService;

        public BudorHubPiClient(ILogger<BudorHubPiClient> logger,
            IWebHostEnvironment environment,
            IConfiguration config,
            ISignalRService signalRService,
            // Anti-pattern, but have currently no other solution
            Container container
        )
        {
            _logger = logger;
            _signalRService = signalRService;
            _container = container;
            RegisterClientMethods();
        }

        public Task ConsoleLogMessage(string message, CancellationToken cancellationToken)
        {
            _logger.LogInformation(message);
            return Task.CompletedTask;
        }

        public Task SendSensorReading(SensorReadingModel model, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task TakeImage(PiCameraSettings settings, CancellationToken cancellationToken)
        {
            // This currently crashes due to mismatch in lifestyle. Needs to be looked into. Commenting out for now...
            // var pictureService = GetPictureService();
            // await pictureService.TakeImageAndUploadAsync(settings, cancellationToken);
            return Task.FromResult(true);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _signalRService.StartWithRetryAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _signalRService.StopAsync(cancellationToken);
        }

        private void RegisterClientMethods()
        {
            var standardCToken = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
            _signalRService.RegisterClientMethod<PiCameraSettings>(nameof(ISignalRService.TakeImage),
                async settings => await TakeImage(settings, standardCToken));
            _signalRService.RegisterClientMethod<string>(nameof(ISignalRService.ConsoleLogMessage),
                message => ConsoleLogMessage(message, standardCToken));
        }

        private IPictureService GetPictureService()
        {
            return _container.GetInstance<IPictureService>();
        }
    }
}