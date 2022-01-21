using System;
using System.Threading;
using Services.Interfaces;

namespace Services
{
    public class ProximityService : IProximityService
    {
        public double GetDistance(CancellationToken cancellationToken)
        {
            // TODO: Replace mock distance with sensor measurement
            var randomDistance = new Random().Next(0, 30);

            return randomDistance;
        }
    }
}