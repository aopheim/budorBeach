using System;
using System.Device.I2c;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using JetBrains.Annotations;
using Quartz;
using rpiDaemon.Models;

namespace rpiDaemon.Jobs
{
    [UsedImplicitly]
    public class GetBme280SensorReadingsJob : IJob
    {
        private readonly ApplicationDbContext _context;

        public GetBme280SensorReadingsJob(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext jobExecutionContext)
        {
            var sensorReadingModel = GetCurrentSensorReadings();

            _context.SensorReadings.Add(sensorReadingModel);
            await _context.SaveChangesAsync(jobExecutionContext.CancellationToken);
        }

        private SensorReadingModel GetCurrentSensorReadings()
        {
            var i2CSettings = new I2cConnectionSettings(1, Bmx280Base.DefaultI2cAddress);
            using var i2CDevice = I2cDevice.Create(i2CSettings);
            using var bme280 = new Bme280(i2CDevice);

            var measurementTime = bme280.GetMeasurementDuration();
            bme280.SetPowerMode(Bmx280PowerMode.Forced);
            Thread.Sleep(measurementTime);

            bme280.TryReadTemperature(out var tempValue);
            bme280.TryReadPressure(out var pressureValue);
            bme280.TryReadHumidity(out var humValue);
            bme280.TryReadAltitude(out var altValue);

            return new SensorReadingModel
            {
                MeasuredAtUtc = DateTime.UtcNow,
                PressureInhPa = pressureValue.Hectopascals,
                TemperatureInDegreesC = tempValue.DegreesCelsius,
                AltitudeInMeters = altValue.Meters,
                RelativeHumidityInPercent = humValue.Percent
            };
        }
    }
}