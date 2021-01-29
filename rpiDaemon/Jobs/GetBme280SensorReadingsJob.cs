using System;
using System.Device.I2c;
using System.Diagnostics;
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
                .WithUrl("http://127.0.0.1:3000/signalr")
                .WithAutomaticReconnect()
                .Build();

            _connection.Closed += async e =>
            {
                _logger.LogError(e, e.Message);
                await Task.Delay(200);
                await _connection.StartAsync();
            };
            _connection.Reconnecting += e =>
            {
                _logger.LogError(e, e.Message);
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

            await SendReadingsToBudorHub(jobExecutionContext.CancellationToken);
        }

        private async Task SendReadingsToBudorHub(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Connecting to hub...");
            await ConnectWithRetryAsync(_connection, cancellationToken);
            _logger.LogInformation("Sending message...");
            await _connection.InvokeAsync("SendMessageToAllClients", "New sensor readings from Pi!", cancellationToken);
            _logger.LogInformation("Message sent");
        }

        private async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token)
        {
            // Keep trying to until we can start or the token is canceled.
            while (true)
                try
                {
                    await connection.StartAsync(token);
                    Debug.Assert(connection.State == HubConnectionState.Connected);
                    _logger.LogInformation("Connection started");
                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    _logger.LogInformation("Cancellation token received");
                    return false;
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "Retrying to restart...");
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    await Task.Delay(5000, token);
                }
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