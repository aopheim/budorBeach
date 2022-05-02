using System.Threading;
using System.Threading.Tasks;
using AudioService.Interfaces;

namespace AudioService
{
    public class AudioService : IAudioService
    {
        public Task<bool> CaptureAudio(CancellationToken cancellationToken)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = @"/C cd C:\Users\AdrianOpheim\Documents\budorBeach\AudioService && py script.py";
            process.StartInfo = startInfo;
            process.Start();

            return Task.FromResult(true);
        }
    }
}