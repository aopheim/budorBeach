using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CameraService
{
    public partial class CameraService
    {
        public async Task StartVideoSurveillance(string fullPath, int secondsToRecord,
            CancellationToken jobCancellationToken)
        {
            _logger.LogWarning("Video surveillance with motion detection is not yet implemented with libcamera.");
            await Task.Delay(0, jobCancellationToken);
        }
    }
}