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
        private bool _isRunning;

        public AudioService()
        {
            _isRunning = false;
        }

        public Task<bool> CaptureAudio(CancellationToken cancellationToken)
        {
            _isRunning = true;
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

            _isRunning = false;
            return Task.FromResult(true);
        }

        public bool IsRunning()
        {
            return _isRunning;
        }
    }
}