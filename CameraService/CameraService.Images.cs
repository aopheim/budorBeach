using System;
using System.Runtime.InteropServices;
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
        private readonly bool _isWindows;

        public CameraService(ILogger<CameraService> logger)
        {
            _logger = logger;
            _cameraIsInUse = false;
            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
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

            _logger.LogInformation("Taking picture");
            await _camera.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
            _cameraIsInUse = false;
        }

        public Task CaptureVideo(string fullPath, int secondsToRecord)
        {
            throw new NotImplementedException();
        }
    }
}