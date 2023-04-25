using System;
using System.Threading.Tasks;
using Innovative.SolarCalculator;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TakePictureJob> _logger;
        private readonly IPictureService _pictureService;
        private readonly IQuartzNetService _quartzNetService;
        private readonly IRpiDaemonSettingsService _rpiDaemonSettingsService;

        public TakePictureJob(IPictureService pictureService, IQuartzNetService quartzNetService,
            IRpiDaemonSettingsService rpiDaemonSettingsService, ILogger<TakePictureJob> logger)
        {
            _pictureService = pictureService;
            _quartzNetService = quartzNetService;
            _rpiDaemonSettingsService = rpiDaemonSettingsService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var rpiDaemonSettings = await _rpiDaemonSettingsService.GetRpiDaemonSettings(context.CancellationToken);
            if (rpiDaemonSettings?.PictureIntervalInMinutes != null && rpiDaemonSettings.PictureIntervalInMinutes > 0)
                _quartzNetService.UpdateTriggerInterval(context,
                    TimeSpan.FromMinutes(rpiDaemonSettings.PictureIntervalInMinutes));

            var now = DateTime.UtcNow;
            var solarTimes = new SolarTimes(now, GlobalConstants.BudorLatitude, GlobalConstants.BudorLongitude);
            var sunrise = solarTimes.Sunrise;
            var sunset = solarTimes.Sunset;

            if (now > sunrise && now < sunset)
                await _pictureService.TakeImageAndUploadAsync(new PiCameraSettings(), context.CancellationToken);
        }
    }
}