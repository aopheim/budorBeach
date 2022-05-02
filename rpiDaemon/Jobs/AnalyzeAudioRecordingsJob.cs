using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.Interfaces;
using Shared;
using Shared.Interfaces;

namespace rpiDaemon.Jobs
{
    public class AnalyzeAudioRecordingsJob : IJob
    {
        private readonly IBirdNetServer _birdNetServer;

        private readonly IFileSystemService _fileSystemService;
        private readonly ILogger<AnalyzeAudioRecordingsJob> _logger;
        private readonly IRepositories _repos;
        private readonly IBirdNetResultConverter _resultConverter;

        public AnalyzeAudioRecordingsJob(ILogger<AnalyzeAudioRecordingsJob> logger,
            IBirdNetResultConverter resultConverter, IFileSystemService fileSystemService, IRepositories repos,
            IBirdNetServer birdNetServer)
        {
            _logger = logger;
            _resultConverter = resultConverter;
            _fileSystemService = fileSystemService;
            _repos = repos;
            _birdNetServer = birdNetServer;
        }

        private static double MinConfidenceLevel => 0.3;

        public async Task Execute(IJobExecutionContext context)
        {
            var recordingsFolderName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? GlobalConstants.AudioRecordingsFolderWindows
                : GlobalConstants.AudioRecordingsFolderLinux;
            var recordingIds = _fileSystemService.GetFileNamesWithoutExtensionInFolder(recordingsFolderName);

            foreach (var recordingId in recordingIds)
            {
                var filePath = recordingsFolderName + recordingId + ".wav";
                var audioFileAsStream = _fileSystemService.GetFileStream(filePath);
                var response =
                    await _birdNetServer.PostAsync(audioFileAsStream?.FileStream,
                        context?.CancellationToken ?? new CancellationToken());
                _logger.LogInformation($"Response from server: {response}");

                var result = _resultConverter.ConvertJson(response);
                if (!result.Results.Any(r => r.Confidence > MinConfidenceLevel))
                {
                    _logger.LogInformation(
                        $"No results with higher confidence than {MinConfidenceLevel}. Deleting recording");
                    _fileSystemService.DeleteFile($"{recordingsFolderName}{recordingId}.wav");
                }
            }
        }
    }
}