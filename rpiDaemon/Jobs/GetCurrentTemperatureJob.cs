using System;
using System.Threading.Tasks;
using Quartz;
using rpiDaemon.Models;

namespace rpiDaemon.Jobs
{
    public class GetCurrentTemperatureJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var temperatureReading = GetCurrentTemperatureReading();

            var sensorReadingModel = new SensorReadingModel
                {MeasuredAtUtc = DateTime.UtcNow, TemperatureInDegreesC = temperatureReading};

            return Task.CompletedTask;
        }

        private double GetCurrentTemperatureReading()
        {
            return new Random().NextDouble();
        }
    }
}