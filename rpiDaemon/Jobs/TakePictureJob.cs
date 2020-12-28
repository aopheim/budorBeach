using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace rpiDaemon.Jobs
{
    public class TakePictureJob : IJob
    {
        private readonly ILogger<TakePictureJob> _logger;

        public TakePictureJob()
        {
        }

        public TakePictureJob(ILogger<TakePictureJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"Taking image at time {DateTime.UtcNow}");
            _logger.LogInformation($"Taking image at time {DateTime.UtcNow}");
            return Task.FromResult(true);
        }
    }
}