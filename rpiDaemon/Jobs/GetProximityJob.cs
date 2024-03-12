using System;
using System.Threading.Tasks;
using Quartz;
using Services;
using Services.Interfaces;

namespace rpiDaemon.Jobs
{
    public class GetProximityJob : IJob
    {
        private readonly IProximityService _proximityService;
        private readonly IBirdPresenceRegistrator _registrator;

        public GetProximityJob(IProximityService proximityService, IBirdPresenceRegistrator registrator)
        {
            _proximityService = proximityService;
            _registrator = registrator;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var currentDistance = _proximityService.GetDistance(context.CancellationToken);
            await _registrator.ReceiveDistanceUpdate(currentDistance, DateTime.UtcNow, context.CancellationToken);
        }
    }
}