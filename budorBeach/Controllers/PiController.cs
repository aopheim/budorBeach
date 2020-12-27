using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MMALSharp;
using MMALSharp.Common;
using MMALSharp.Handlers;

namespace budorBeach.Controllers
{
    [Route("api/PiController")]
    [ApiController]
    public class PiController : ControllerBase
    {
        [HttpPost]
        public async Task TakePicture()
        {
            var cam = MMALCamera.Instance;

            using (var imgCaptureHandler = new ImageStreamCaptureHandler("/home/pi/images/", "jpg"))
            {
                await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
            }

            cam.Cleanup();
        }
    }
}