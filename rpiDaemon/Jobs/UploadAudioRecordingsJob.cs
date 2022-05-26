using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.Interfaces;

namespace rpiDaemon.Jobs
{
    public class UploadAudioRecordingsJob : IJob
    {
        private readonly IAudioUploader _audioUploader;
        private readonly ILogger<UploadAudioRecordingsJob> _logger;

        public UploadAudioRecordingsJob(ILogger<UploadAudioRecordingsJob> logger, IAudioUploader audioUploader)
        {
            _logger = logger;
            _audioUploader = audioUploader;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (_audioUploader.IsRunning())
            {
                _logger.LogInformation("Audio uploader already running. Skipping...");
                return;
            }

            await _audioUploader.StartUpload(context.CancellationToken);
        }
    }
}