using System.Threading.Tasks;
using CameraService.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace rpiDaemon.Jobs
{
    public class StreamVideoJob : IJob
    {
        private readonly ICameraService _cameraService;
        private readonly ILogger<StreamVideoJob> _logger;

        public StreamVideoJob(ICameraService cameraService, ILogger<StreamVideoJob> logger)
        {
            _cameraService = cameraService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (_cameraService.CameraIsInUse())
            {
                _logger.LogInformation("Camera is already in use. Can not start streaming");
                return;
            }

            await _cameraService.StartVideoStream(context.CancellationToken);
        }
    }
}