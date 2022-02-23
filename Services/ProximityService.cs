using System;
using System.Threading;
using Iot.Device.Hcsr04;
using Services.Interfaces;

namespace Services
{
    public class ProximityService : IProximityService
    {
        // TODO: Set correct pin number
        private const int TriggerPin = 1;
        private const int EchoPin = 2;

        public double GetDistance(CancellationToken cancellationToken)
        {
            var hcsr04 = new Hcsr04(TriggerPin, EchoPin);
            // return hcsr04.Distance.Centimeters;

            // TODO: Replace mock distance with sensor measurement
            var randomDistance = new Random().Next(0, 30);

            return randomDistance;
        }
    }
}