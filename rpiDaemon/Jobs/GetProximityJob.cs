using System;
using System.Threading.Tasks;
using Quartz;
using Services;
using Services.Interfaces;

namespace rpiDaemon.Jobs
{
    public class GetProximityJob : IJob
    {
        private readonly IBirdPresenceRegistrator _registrator;
        private readonly IProximityService _proximityService;

        public GetProximityJob(IProximityService proximityService, IBirdPresenceRegistrator registrator)
        {
            _proximityService = proximityService;
            _registrator = registrator;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var currentDistance = _proximityService.GetDistance(context.CancellationToken);
            _registrator.ReceiveDistanceUpdate(currentDistance, DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}