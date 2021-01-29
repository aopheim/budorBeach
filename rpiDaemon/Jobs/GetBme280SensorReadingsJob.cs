using System;
using System.Device.I2c;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Quartz;
using rpiDaemon.Models;

namespace rpiDaemon.Jobs
{
    [UsedImplicitly]
    public class GetBme280SensorReadingsJob : IJob
    {
        private readonly HubConnection _connection;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetBme280SensorReadingsJob> _logger;

        public GetBme280SensorReadingsJob(ApplicationDbContext context, ILogger<GetBme280SensorReadingsJob> logger)
        {
            _context = context;
            _logger = logger;
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:3001/budorhub")
                .Build();
            _connection.Closed += async error =>
            {
                await Task.Delay(200);
                await _connection.StartAsync();
            };
        }

        public async Task Execute(IJobExecutionContext jobExecutionContext)
        {
            var sensorReadingModel = GetCurrentSensorReadings();
            _logger.LogInformation($"{JsonSerializer.Serialize(sensorReadingModel)}");

            _context.SensorReadings.Add(sensorReadingModel);
            await _context.SaveChangesAsync(jobExecutionContext.CancellationToken);

            await SendReadingsToBudorHub(jobExecutionContext.CancellationToken);
        }

        private async Task SendReadingsToBudorHub(CancellationToken cancellationToken)
        {
            await _connection.StartAsync(cancellationToken);
            await _connection.InvokeAsync("SendMessageToAllClients", "New sensor readings from Pi!", cancellationToken);
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