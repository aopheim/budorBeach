using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace Services
{
    public class ExternalSingletonProcess : IExternalSingletonProcess
    {
        private readonly ILogger<ExternalProcess> _logger;

        public ExternalSingletonProcess(ILogger<ExternalProcess> logger)
        {
            _logger = logger;
        }

        public Process StartExternalSingletonProcess(bool isWindows, string commandLineArguments,
            CancellationToken cancellationToken)
        {
            return ExternalProcessHelper.StartProcess(isWindows, commandLineArguments, _logger);
        }
    }

    public class ExternalProcess : IExternalProcess
    {
        private readonly ILogger<ExternalProcess> _logger;

        public ExternalProcess(ILogger<ExternalProcess> logger)
        {
            _logger = logger;
        }

        public Process StartExternalProcess(bool isWindows, string commandLineArguments,
            CancellationToken cancellationToken)
        {
            return ExternalProcessHelper.StartProcess(isWindows, commandLineArguments, _logger);
        }
    }

    internal static class ExternalProcessHelper
    {
        public static Process StartProcess(bool isWindows, string commandLineArguments, ILogger logger)
        {
            var process = new Process();

            var startInfo = new ProcessStartInfo
            {
                FileName = isWindows ? "cmd.exe" : "/bin/bash",
                Arguments = commandLineArguments,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true
            };
            process.StartInfo = startInfo;
            process.OutputDataReceived += (sender, args) =>
            {
                if (args?.Data != null)
                    logger.LogInformation(args.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            return process;
        }
    }
}