using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Handlers;

namespace budorBeach.Controllers
{
    [Route("api/PiController")]
    [ApiController]
    public class PiController : ControllerBase
    {
        private readonly ILogger<PiController> _logger;

        public PiController(ILogger<PiController> logger)
        {
            _logger = logger;
        }

        [HttpPost("takePicture")]
        public async Task TakePicture()
        {
            _logger.LogInformation("Taking picture");
            var cam = MMALCamera.Instance;

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg"))
            {
                await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
            }

            cam.Cleanup();
        }
    }
}