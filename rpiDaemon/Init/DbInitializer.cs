using System;
using System.Collections.Generic;
using rpiDaemon.Models;

namespace rpiDaemon.Init
{
    public class DbInitializer
    {
        public static void Initialize(SensorContext context)
        {
            context.Database.EnsureCreated();

            var readings = new List<SensorReadingModel>();

            for (var i = 0; i < 50; i++)
            {
                var randomDouble = new Random().NextDouble();
                readings.Add(new SensorReadingModel
                    {MeasuredAtUtc = DateTime.UtcNow, TemperatureInDegreesC = randomDouble});
            }

            foreach (var reading in readings) context.SensorReadings.Add(reading);

            context.SaveChanges();
        }
    }
}