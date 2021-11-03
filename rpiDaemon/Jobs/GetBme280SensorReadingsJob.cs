using System;
using System.Device.I2c;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Shared;
using Shared.Models;
using Shared.SignalR;

namespace rpiDaemon.Jobs
{
    [UsedImplicitly]
    public class GetBme280SensorReadingsJob : IJob
    {
        private readonly HubConnection _connection;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly Fixture _fixture;
        private readonly ILogger<GetBme280SensorReadingsJob> _logger;

        public GetBme280SensorReadingsJob(ApplicationDbContext context, ILogger<GetBme280SensorReadingsJob> logger,
            IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
            _fixture = new Fixture();

            var urlToUse = environment.IsProduction()
                ? GlobalConstants.ProductionHubUrl
                : GlobalConstants.DevelopmentHubUrl;
            _connection = SignalRHelper.GetHubConnection(urlToUse);
            SignalRHelper.SetupEventsForDebuggingConnection(logger, _connection);
        }

        private static TimeSpan DbPushInterval => TimeSpan.FromMinutes(2);

        public async Task Execute(IJobExecutionContext jobExecutionContext)
        {
            var sensorReadingModel = GetCurrentSensorReadings();
            _logger.LogInformation($"{JsonSerializer.Serialize(sensorReadingModel)}");

            var lastDbPushInUtc = _context.SensorReadings.OrderByDescending(m => m.MeasuredAtUtc).FirstOrDefault()
                ?.MeasuredAtUtc;
            if (lastDbPushInUtc == null ||
                sensorReadingModel.MeasuredAtUtc.Subtract(lastDbPushInUtc.Value) > DbPushInterval ||
                _environment.IsDevelopment())
            {
                _context.SensorReadings.Add(sensorReadingModel);
                await _context.SaveChangesAsync(jobExecutionContext.CancellationToken);
            }

            await PushReadingsToBudorHub(sensorReadingModel, jobExecutionContext.CancellationToken);
            await _connection.StopAsync(jobExecutionContext.CancellationToken);
            await _connection.DisposeAsync();
        }

        private async Task PushReadingsToBudorHub(SensorReadingModel model, CancellationToken cancellationToken)
        {
            await SignalRHelper.StartWithRetryAsync(_connection, cancellationToken);
            await _connection.InvokeAsync(nameof(BudorHub.SendSensorReadingModelToWebClient), model,
                cancellationToken);
            await _connection.InvokeAsync(nameof(BudorHub.SendMessageToAllClients),
                $"{JsonSerializer.Serialize(model)}", cancellationToken);
        }

        private SensorReadingModel GetCurrentSensorReadings()
        {
            if (_environment.IsDevelopment())
            {
                var model = _fixture.Create<SensorReadingModel>();
                model.MeasuredAtUtc = DateTime.UtcNow;
                model.Id = default;
                return model;
            }

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