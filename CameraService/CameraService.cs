using System;
using System.Threading.Tasks;
using CameraService.Interfaces;
using Microsoft.Extensions.Logging;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Handlers;
using Shared.PiCameraSettings;

namespace CameraService
{
    public class CameraService : ICameraService
    {
        private readonly ILogger<CameraService> _logger;
        private MMALCamera _camera;

        public CameraService(ILogger<CameraService> logger)
        {
            _logger = logger;
            try
            {
                _camera = MMALCamera.Instance;
            }
            catch (Exception e)
            {
                _logger.LogError("Setting MMALCamera instance throws exception", e);
                _camera = default;
            }
        }

        public async Task TakeImage(string fullPath, PiCameraSettings settings)
        {
            var camera = MMALCamera.Instance;
            using var imgCaptureHandler = new ImageStreamCaptureHandler(fullPath);

            MMALCameraConfig.ISO = settings.Iso;
            MMALCameraConfig.ShutterSpeed = settings.ShutterTime;

            camera.ConfigureCameraSettings();

            await camera.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
        }

        public Task CaptureVideo(string fullPath, int secondsToRecord)
        {
            throw new NotImplementedException();
        }
    }
}