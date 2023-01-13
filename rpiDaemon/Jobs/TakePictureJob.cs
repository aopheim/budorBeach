using System;
using System.Threading.Tasks;
using Innovative.SolarCalculator;
using JetBrains.Annotations;
using Quartz;
using Services.Interfaces;
using Shared;
using Shared.PiCameraSettings;

namespace rpiDaemon.Jobs
{
    [DisallowConcurrentExecution]
    [UsedImplicitly]
    public class TakePictureJob : IJob
    {
        private readonly IPictureService _pictureService;

        public TakePictureJob(IPictureService pictureService)
        {
            _pictureService = pictureService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            var solarTimes = new SolarTimes(now, GlobalConstants.BudorLatitude, GlobalConstants.BudorLongitude);
            var sunrise = solarTimes.Sunrise;
            var sunset = solarTimes.Sunset;

            if (now > sunrise && now < sunset)
                await _pictureService.TakeImageAndUploadAsync(new PiCameraSettings(), context.CancellationToken);
        }
    }
}