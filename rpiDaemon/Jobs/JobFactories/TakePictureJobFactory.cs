using System;
using CameraService.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace rpiDaemon.Jobs.JobFactories
{
    public class TakePictureJobFactory : IJobFactory
    {
        private readonly ICameraService _cameraService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<TakePictureJob> _logger;

        public TakePictureJobFactory(ICameraService cameraService, ILogger<TakePictureJob> logger,
            IConfiguration config, IWebHostEnvironment environment)
        {
            _cameraService = cameraService;
            _logger = logger;
            _config = config;
            _environment = environment;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return new TakePictureJob(_logger, _config, _environment, _cameraService);
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}