using System.Threading.Tasks;
using Services.Interfaces;

namespace Services
{
    public class BirdNetServer : IBirdNetServer
    {
        public Task<bool> StartBirdNETServer()
        {
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = @"/C cd C:\repos\BirdNET-Analyzer && py server.py";
            process.StartInfo = startInfo;

            return Task.FromResult(process.Start());
        }
    }
}