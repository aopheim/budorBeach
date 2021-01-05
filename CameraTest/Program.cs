using System;
using System.Threading.Tasks;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Ports;
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
            for (var i = 0; i < 5; i++)
            {
                var cam = MMALCamera.Instance;
                Console.WriteLine("Acquired instance");
                MMALCameraConfig.Debug = true;

                MMALCameraConfig.ISO = 800;
                MMALCameraConfig.ShutterSpeed = 2000000;

                using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/test.jpg"))
                using (var imgEncoder = new MMALImageEncoder())
                using (var nullSink = new MMALNullSinkComponent())
                {
                    cam.ConfigureCameraSettings();

                    var portConfig = new MMALPortConfig(MMALEncoding.JPEG, MMALEncoding.I420, 90);

                    imgEncoder.ConfigureOutputPort(portConfig, imgCaptureHandler);

                    cam.Camera.StillPort.ConnectTo(imgEncoder);
                    cam.Camera.PreviewPort.ConnectTo(nullSink);

                    await Task.Delay(2000);

                    await cam.ProcessAsync(cam.Camera.StillPort);
                }

                Console.WriteLine($"Picture taken at {now}");
            }
        }
    }
}