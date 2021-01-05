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
            Console.WriteLine("Taking 10 pictures...");
            for (var i = 0; i < 5; i++)
            {
                var now = DateTime.UtcNow;
                var folderName = DateTimeParser.GetFolderName(now);
                var fileName = DateTimeParser.GetFileName(now);
                var fullPath = $"/home/pi/images/{folderName}/{fileName}.jpg";

                var cam = MMALCamera.Instance;
                Console.WriteLine("Acquired instance");
                MMALCameraConfig.Debug = true;

                //MMALCameraConfig.ISO = 800;
                //MMALCameraConfig.ShutterSpeed = 2000000;

                using var imgCaptureHandler = new ImageStreamCaptureHandler(fullPath);
                cam.ConfigureCameraSettings();
                Console.WriteLine("Settings configured");
                await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);

                Console.WriteLine($"Picture taken at {now}");
            }
        }
    }
}