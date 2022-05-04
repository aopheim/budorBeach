using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AudioService.Interfaces;
using Shared;

namespace AudioService
{
    public class AudioService : IAudioService
    {
        public Task<bool> CaptureAudio(CancellationToken cancellationToken)
        {
            Process process = new();
            ProcessStartInfo startInfo = new();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            var filePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? GlobalConstants.AudioServiceFolderWindows
                : GlobalConstants.AudioServiceFolderLinux;
            startInfo.Arguments = @$"/C cd {filePath} && py takeAudioRecordings.py";
            process.StartInfo = startInfo;
            process.Start();

            return Task.FromResult(true);
        }
    }
}