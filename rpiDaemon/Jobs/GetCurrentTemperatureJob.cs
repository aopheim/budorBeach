using System;
using System.Threading.Tasks;
using Quartz;
using rpiDaemon.Models;

namespace rpiDaemon.Jobs
{
    public class GetCurrentTemperatureJob : IJob
    {
        private readonly ApplicationDbContext _context;

        public GetCurrentTemperatureJob(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var temperatureReading = GetCurrentTemperatureReading();

            var sensorReadingModel = new SensorReadingModel
                {MeasuredAtUtc = DateTime.UtcNow, TemperatureInDegreesC = temperatureReading};
            await _context.SensorReadings.AddAsync(sensorReadingModel);

            await _context.SaveChangesAsync(context.CancellationToken);
        }

        private double GetCurrentTemperatureReading()
        {
            return new Random().NextDouble();
        }
    }
}