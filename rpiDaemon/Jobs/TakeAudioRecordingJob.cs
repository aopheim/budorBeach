using System.Threading.Tasks;
using AudioService.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace rpiDaemon.Jobs
{
    public class TakeAudioRecordingJob : IJob
    {
        private readonly IAudioService _audioService;
        private readonly ILogger<TakeAudioRecordingJob> _logger;

        public TakeAudioRecordingJob(IAudioService audioService, ILogger<TakeAudioRecordingJob> logger)
        {
            _audioService = audioService;
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            if (_audioService.IsRunning())
            {
                _logger.LogInformation("Audio recording already running. Skipping");
                return Task.CompletedTask;
            }

            _audioService.CaptureAudio(context.CancellationToken);
            return Task.CompletedTask;
        }
    }
}