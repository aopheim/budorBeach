using System;
using System.Device.I2c;
using System.Diagnostics;
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
using rpiDaemon.Models;
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

            var developmentUrl = "http://localhost:3000/budorhub";
            // TODO: Change to production url
            var productionUrl = "http://localhost:3000/budorhub";
            _connection = new HubConnectionBuilder()
                .WithUrl(environment.IsProduction() ? productionUrl : developmentUrl)
                .WithAutomaticReconnect()
                .Build();

            _connection.Closed += async _ =>
            {
                _logger.LogError(_, _.Message);
                await Task.Delay(200);
                await _connection.StartAsync();
            };
            _connection.Reconnecting += _ =>
            {
                _logger.LogError(_, _.Message);
                Debug.Assert(_connection.State == HubConnectionState.Reconnecting);
                return Task.CompletedTask;
            };
            _connection.Reconnected += message =>
            {
                _logger.LogInformation(message);
                Debug.Assert(_connection.State == HubConnectionState.Connected);
                return Task.CompletedTask;
            };
        }

        public async Task Execute(IJobExecutionContext jobExecutionContext)
        {
            var sensorReadingModel = GetCurrentSensorReadings();
            _logger.LogInformation($"{JsonSerializer.Serialize(sensorReadingModel)}");

            _context.SensorReadings.Add(sensorReadingModel);
            await _context.SaveChangesAsync(jobExecutionContext.CancellationToken);

            await PushReadingsToBudorHub(sensorReadingModel, jobExecutionContext.CancellationToken);
        }

        private async Task PushReadingsToBudorHub(SensorReadingModel model, CancellationToken cancellationToken)
        {
            await SignalRHelper.ConnectWithRetryAsync(_connection, cancellationToken);
            await _connection.InvokeAsync("SendSensorReadingModelToWebClient", model,
                cancellationToken);
        }

        private SensorReadingModel GetCurrentSensorReadings()
        {
            if (_environment.IsDevelopment())
            {
                var model = _fixture.Create<SensorReadingModel>();
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