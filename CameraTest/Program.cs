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

            Console.WriteLine("Taking 10 pictures...");
            for (var i = 0; i < 10; i++)
            {
                var camera = MMALCamera.Instance;
                Console.WriteLine("Acquired instance");
                MMALCameraConfig.Debug = true;

                try
                {
                    using var imageCaptureHandler = new ImageStreamCaptureHandler(fullPath);
                    Console.WriteLine("Got ImageStreamCaptureHandler");
                    await camera.TakePicture(imageCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                camera.Cleanup();
                Console.WriteLine($"Picture taken at {now}");
            }
        }
    }
}