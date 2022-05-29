using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CameraService.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Shared;

namespace rpiDaemon.Jobs
{
    public class StartVideoSurveillanceJob : IJob
    {
        private readonly ICameraService _cameraService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<StartVideoSurveillanceJob> _logger;

        public StartVideoSurveillanceJob(ICameraService cameraService, ILogger<StartVideoSurveillanceJob> logger,
            IWebHostEnvironment environment)
        {
            _cameraService = cameraService;
            _logger = logger;
            _environment = environment;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (_cameraService.CameraIsInUse())
            {
                _logger.LogInformation("Camera already in use. Skipping");
                return;
            }

            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (isWindows)
            {
                _logger.LogInformation("Skipping video surveillance because running on Windows");
                return;
            }

            var path = isWindows
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BudorBeach\videos"
                : GlobalConstants.VideoRecordingsFolderLinux;

            await _cameraService.StartVideoSurveillance(path, 7, context.CancellationToken);
        }
    }
}