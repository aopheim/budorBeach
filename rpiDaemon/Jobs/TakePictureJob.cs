using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Handlers;
using Quartz;

namespace rpiDaemon.Jobs
{
    [UsedImplicitly]
    public class TakePictureJob : IJob
    {
        private readonly ILogger<TakePictureJob> _logger;

        public TakePictureJob()
        {
        }

        public TakePictureJob(ILogger<TakePictureJob> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var now = DateTime.UtcNow;
                var folderName = $"{now.Year}-{now.Month}-{now.Day}";

                var cam = MMALCamera.Instance;

                using (var imgCaptureHandler = new ImageStreamCaptureHandler($"/home/pi/images/{folderName}", "jpg"))
                {
                    await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
                }

                cam.Cleanup();
            }
            catch (Exception e)
            {
                if (e is DllNotFoundException) _logger.LogError("Raspberry Pi camera not found");
                else
                    throw;
            }
        }
    }
}