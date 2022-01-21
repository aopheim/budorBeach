using System;
using System.Threading.Tasks;
using Quartz;
using Services;
using Services.Interfaces;

namespace rpiDaemon.Jobs
{
    public class GetProximityJob : IJob
    {
        private readonly IBirdPresenceCalculator _calc;
        private readonly IProximityService _proximityService;

        public GetProximityJob(IProximityService proximityService, IBirdPresenceCalculator calc)
        {
            _proximityService = proximityService;
            _calc = calc;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var currentDistance = _proximityService.GetDistance(context.CancellationToken);
            _calc.ReceiveDistanceUpdate(currentDistance, DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}