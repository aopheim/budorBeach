using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.PiCameraSettings;

namespace Services.Interfaces;

public interface IPictureService
{
    [Obsolete("MMALSharp will not work on newest versions of Raspberry Pi OS, making this unusable")]
    Task TakeImageAndUploadAsync(PiCameraSettings settings, CancellationToken cancellationToken);
}