using System;
using System.Threading;
using Iot.Device.Hcsr04;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Services.Interfaces;

namespace Services
{
    public class ProximityService : IProximityService
    {
        // TODO: Set correct pin number
        private const int TriggerPin = 1;
        private const int EchoPin = 2;
        private readonly IWebHostEnvironment _environment;

        public ProximityService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public double GetDistance(CancellationToken cancellationToken)
        {
            if (_environment.IsDevelopment())
                return new Random().Next(0, 30);

            var distanceSensor = new Hcsr04(TriggerPin, EchoPin);
            return distanceSensor.Distance.Centimeters;
        }
    }
}