using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.Interfaces;

namespace rpiDaemon.Jobs
{
    public class AnalyzeAudioRecordingsJob : IJob
    {
        private readonly IBirdRecordingAnalyzer _recordingAnalyzer;
        private readonly ILogger<AnalyzeAudioRecordingsJob> _logger;

        public AnalyzeAudioRecordingsJob(IBirdRecordingAnalyzer recordingAnalyzer, ILogger<AnalyzeAudioRecordingsJob> logger)
        {
            _recordingAnalyzer = recordingAnalyzer;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (_recordingAnalyzer.IsRunning())
            {
                _logger.LogInformation("Analyzer already running. Skipping");
                return;
            }

            await _recordingAnalyzer.RunAnalyzer(context.CancellationToken);
        }
    }
}