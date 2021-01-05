using System;
using System.Threading.Tasks;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Handlers;
using rpiDaemon.DateTimeHelpers;

namespace CameraTest
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var now = DateTime.UtcNow;
            var folderName = DateTimeParser.GetFolderName(now);
            var fileName = DateTimeParser.GetFileName(now);
            var fullPath = $"/home/pi/images/{folderName}/{fileName}.jpg";

            for (var i = 0; i < 10; i++)
            {
                var camera = MMALCamera.Instance;
                MMALCameraConfig.Debug = true;

                using var imgCaptureHandler = new ImageStreamCaptureHandler(fullPath);
                await camera.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
            }
        }
    }
}