using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CameraService.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;
using Shared;

namespace rpiDaemon.Jobs
{
    public class StartVideoSurveillanceJob : IJob
    {
        private readonly ICameraService _cameraService;
        private readonly ILogger<StartVideoSurveillanceJob> _logger;

        public StartVideoSurveillanceJob(ICameraService cameraService, ILogger<StartVideoSurveillanceJob> logger)
        {
            _cameraService = cameraService;
            _logger = logger;
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            if (_cameraService.CameraIsInUse())
            {
                _logger.LogInformation("Analyzer already running. Skipping");
                return;
            }

            var path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BudorBeach\videos"
                : GlobalConstants.VideoRecordingsFolderLinux;

            await _cameraService.StartVideoSurveillance(path, 7);

        }
    }
}