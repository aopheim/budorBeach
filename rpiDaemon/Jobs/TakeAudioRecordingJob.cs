using System.Threading.Tasks;
using AudioService.Interfaces;
using Quartz;

namespace rpiDaemon.Jobs
{
    public class TakeAudioRecordingJob : IJob
    {
        private readonly IAudioService _audioService;

        public TakeAudioRecordingJob(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _audioService.CaptureAudio(context.CancellationToken);
            return Task.CompletedTask;
        }
    }
}