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
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var filePath = isWindows
                ? GlobalConstants.AudioServiceFolderWindows
                : GlobalConstants.AudioServiceFolderLinux;
            var fileName = isWindows ? "recordAudioWindows.py" : "recordAudioLinux.py";
            startInfo.Arguments = @$"/C cd {filePath} && py {fileName}";
            process.StartInfo = startInfo;
            process.Start();

            return Task.FromResult(true);
        }
    }
}