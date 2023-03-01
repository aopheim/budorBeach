using System.Threading.Tasks;
using AudioService.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace rpiDaemon.Jobs
{
    public class CaptureAudioContinuouslyJob : IJob
    {
        private readonly IAudioService _audioService;
        private readonly ILogger<CaptureAudioContinuouslyJob> _logger;

        public CaptureAudioContinuouslyJob(IAudioService audioService, ILogger<CaptureAudioContinuouslyJob> logger)
        {
            _audioService = audioService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (_audioService.IsRunning())
            {
                _logger.LogInformation("Audio recording already running. Skipping");
                return;
            }

            await _audioService.CaptureAudioContinuously(context.CancellationToken);
        }
    }
}