using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Services
{
    public class BirdPresenceCalculator : IBirdPresenceCalculator
    {
        private const double BirdPresenceMaxDistance = 30;
        private const double MinimumChangeInDistance = 3;
        private readonly Container _container;
        private Tuple<double, DateTime> _previousDistanceMeasurement;
        private TimeSpan _triggerIntervalAtBirdPresence = TimeSpan.FromSeconds(2);
        private TimeSpan _triggerIntervalAtRest = TimeSpan.FromSeconds(10);

        public BirdPresenceCalculator(
            Container container
        )
        {
            _container = container;
        }

        public Task ReceiveDistanceUpdate(double currentDistance, DateTime measuredAt)
        {
            using var scope = AsyncScopedLifestyle.BeginScope(_container);
            var repos = scope.GetRequiredService<IRepositories>();
            
            var isPresent = BirdIsPresent(currentDistance);
            var isMovement = IsMovement(currentDistance);

            var lastReg = repos.BirdPresenceReadings.GetLatestRegistration();
            var currentStatus = _previousDistanceMeasurement = new Tuple<double, DateTime>(currentDistance, measuredAt);
            return Task.CompletedTask;
        }

        private bool BirdIsPresent(double currentDistance)
        {
            return BirdPresenceMaxDistance - currentDistance > 0;
        }

        private bool IsMovement(double currentDistance)
        {
            if (_previousDistanceMeasurement == null)
                return false;
            return Math.Abs(currentDistance - _previousDistanceMeasurement.Item1) > MinimumChangeInDistance;
        }
    }

    public interface IBirdPresenceCalculator
    {
        Task ReceiveDistanceUpdate(double currentDistance, DateTime measuredAt);
    }
}