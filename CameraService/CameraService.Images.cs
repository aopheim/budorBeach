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
    public partial class CameraService : ICameraService
    {
        private readonly ILogger<CameraService> _logger;
        private MMALCamera _camera;
        private bool _cameraIsInUse;

        public CameraService(ILogger<CameraService> logger)
        {
            _logger = logger;
            _cameraIsInUse = false;
        }

        public bool CameraIsInUse()
        {
            return _cameraIsInUse;
        }

        public async Task TakeImage(string fullPath, PiCameraSettings settings)
        {
            _cameraIsInUse = true;
            try
            {
                _camera = MMALCamera.Instance;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Setting MMALCamera instance throws exception");
                return;
            }

            using var imgCaptureHandler = new ImageStreamCaptureHandler(fullPath);

            MMALCameraConfig.ISO = settings.Iso;
            MMALCameraConfig.ShutterSpeed = settings.ShutterTime;
            _camera.ConfigureCameraSettings();

            await _camera.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
            _cameraIsInUse = false;
        }

        public Task CaptureVideo(string fullPath, int secondsToRecord)
        {
            throw new NotImplementedException();
        }
    }
}