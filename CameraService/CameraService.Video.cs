using System;
using System.Threading;
using System.Threading.Tasks;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Common.Utility;
using MMALSharp.Components;
using MMALSharp.Handlers;
using MMALSharp.Ports;
using MMALSharp.Ports.Outputs;
using MMALSharp.Processors.Motion;

namespace CameraService
{
    public partial class CameraService
    {
        private readonly TimeSpan _videoSurveillanceLength = TimeSpan.FromMinutes(60);

        public async Task StartVideoSurveillance(string fullPath, int secondsToRecord)
        {
            // Assumes the camera has been configured.
            var cam = MMALCamera.Instance;

            // h.264 requires key frames for the circular buffer capture handler.
            MMALCameraConfig.InlineHeaders = true;

            using (var videoCaptureHandler =
                new CircularBufferCaptureHandler(4000000, fullPath, "h264"))
            using (var motionCaptureHandler = new FrameBufferCaptureHandler())
            using (var resizer = new MMALIspComponent())
            using (var splitter = new MMALSplitterComponent())
            using (var videoEncoder = new MMALVideoEncoder())
            {
                splitter.ConfigureInputPort(new MMALPortConfig(MMALEncoding.OPAQUE, MMALEncoding.I420),
                    cam.Camera.VideoPort, null);
                videoEncoder.ConfigureOutputPort(
                    new MMALPortConfig(MMALEncoding.H264, MMALEncoding.I420, 0, MMALVideoEncoder.MaxBitrateLevel4),
                    videoCaptureHandler);

                // As with the basic example, the resizer sends 640 x 480 raw frames to the motion detection handler.
                resizer.ConfigureOutputPort<VideoPort>(0,
                    new MMALPortConfig(MMALEncoding.RGB24, MMALEncoding.RGB24, width: 640, height: 480),
                    motionCaptureHandler);

                cam.Camera.VideoPort.ConnectTo(splitter);
                splitter.Outputs[0].ConnectTo(resizer);
                splitter.Outputs[1].ConnectTo(videoEncoder);

                // Camera warm-up.
                await Task.Delay(2000);

                // We'll use the default settings for this example.
                var motionConfig = new MotionConfig(new MotionAlgorithmRGBDiff());

                // Duration of the motion-detection operation.
                var stoppingToken = new CancellationTokenSource(_videoSurveillanceLength);
                Console.WriteLine($"Detecting motion for {_videoSurveillanceLength.Seconds} seconds.");

                await cam.WithMotionDetection(
                        motionCaptureHandler,
                        motionConfig,
                        // This callback will be invoked when motion has been detected.
                        async () =>
                        {
                            // When motion is detected, temporarily disable notifications
                            motionCaptureHandler.DisableMotionDetection();
                            Console.WriteLine(
                                $"\n     {DateTime.Now:hh\\:mm\\:ss} Motion detected, recording for {secondsToRecord} seconds.");

                            // When the recording period expires, stop recording and re-enable capture
                            var stopRecording = new CancellationTokenSource();
                            stopRecording.Token.Register(() =>
                            {
                                Console.WriteLine($"     {DateTime.Now:hh\\:mm\\:ss} ...recording stopped.");
                                motionCaptureHandler.EnableMotionDetection();

                                // Calling split will close the h.264 file stream and open another file to
                                // store new circular buffer data while we wait for another motion event.
                                videoCaptureHandler.StopRecording();
                                videoCaptureHandler.Split();
                            });

                            // Start the recording countdown
                            stopRecording.CancelAfter(secondsToRecord * 1000);

                            // Record until the duration passes or the overall motion detection token expires
                            await Task.WhenAny(
                                // Calling StartRecording saves the contents of the circular buffer, then begins appending new
                                // video frames to the buffer until StopRecording is called. The first argument is an optional
                                // initialization Action, which in this case ensures the h.264 stream emits an IFrame.
                                videoCaptureHandler.StartRecording(videoEncoder.RequestIFrame, stopRecording.Token),
                                stoppingToken.Token.AsTask()
                            );

                            // If the awaiter above exited because the overall stoppingToken
                            // has expired, ensure we also terminate the ongoing recording.
                            if (!stopRecording.IsCancellationRequested) stopRecording.Cancel();
                        })
                    .ProcessAsync(cam.Camera.VideoPort, stoppingToken.Token);
            }

            cam.Cleanup();
        }
    }
}