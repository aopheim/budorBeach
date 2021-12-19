using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models;

namespace rpiDaemon.Init
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.SensorReadings.Any()) return;
            var readings = new List<SensorReadingModel>();

            for (var i = 0; i < 50; i++)
            {
                var randomDouble = new Random().NextDouble();
                readings.Add(new SensorReadingModel
                    { MeasuredAtUtc = DateTime.UtcNow, TemperatureInDegreesC = randomDouble });
            }

            foreach (var reading in readings) context.SensorReadings.Add(reading);

            context.SaveChanges();
        }
    }
}