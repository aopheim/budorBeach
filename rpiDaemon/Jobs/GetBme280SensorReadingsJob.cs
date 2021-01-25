using System;
using System.Device.I2c;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Quartz;

namespace rpiDaemon.Jobs
{
    public class GetBme280SensorReadingsJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
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

            Console.WriteLine($"Temperature: {tempValue.DegreesCelsius:0.#}\u00B0C");
            Console.WriteLine($"Pressure: {pressureValue.Hectopascals:#.##} hPa");
            Console.WriteLine($"Relative humidity: {humValue.Percent:#.##}%");
            Console.WriteLine($"Estimated altitude: {altValue.Meters:#} m");

            return Task.CompletedTask;
        }
    }
}