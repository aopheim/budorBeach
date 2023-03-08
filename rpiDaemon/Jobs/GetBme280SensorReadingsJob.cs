using System;
using System.Device.I2c;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using DataAccess.EFCore;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Shared.Models;
using Shared.SignalR;

namespace rpiDaemon.Jobs
{
    [DisallowConcurrentExecution]
    [UsedImplicitly]
    public class GetBme280SensorReadingsJob : IJob
    {
        public static readonly TimeSpan ActiveStateTriggerInterval = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan FailedStateTriggerInterval = TimeSpan.FromHours(1);
        private readonly BudorDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly Fixture _fixture;
        private readonly ILogger<GetBme280SensorReadingsJob> _logger;
        private readonly ISignalRService _signalRService;

        public GetBme280SensorReadingsJob(BudorDbContext context, ILogger<GetBme280SensorReadingsJob> logger,
            IWebHostEnvironment environment, ISignalRService signalRService)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
            _signalRService = signalRService;
            _fixture = new Fixture();
        }

        private static TimeSpan DbPushInterval => TimeSpan.FromMinutes(2);

        public async Task Execute(IJobExecutionContext jobExecutionContext)
        {
            SensorReadingModel sensorReadingModel = null;
            try
            {
                sensorReadingModel = GetCurrentSensorReadings();
            }
            catch (IOException)
            {
                _logger.LogWarning(
                    $"Failed to read bme280 sensor. Setting trigger interval to {FailedStateTriggerInterval}");
                SetTriggerToHaveInterval(jobExecutionContext, FailedStateTriggerInterval);
            }

            if (sensorReadingModel == null)
                return;
            SetTriggerToHaveInterval(jobExecutionContext, ActiveStateTriggerInterval);
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

            await _signalRService.SendSensorReading(sensorReadingModel, jobExecutionContext.CancellationToken);
            await _signalRService.ConsoleLogMessage($"{JsonSerializer.Serialize(sensorReadingModel)}",
                jobExecutionContext.CancellationToken);
        }

        private void SetTriggerToHaveInterval(IJobExecutionContext context, TimeSpan newInterval)
        {
            var oldTrigger = context.Trigger;
            var builder = oldTrigger.GetTriggerBuilder();
            var nextFireTime = oldTrigger.GetNextFireTimeUtc();
            if (nextFireTime.HasValue &&
                DatesAreClose(nextFireTime.Value.UtcDateTime, DateTime.UtcNow.Add(newInterval))) return;

            var firstNewTriggerTime = DateTimeOffset.UtcNow.Add(newInterval);
            var newTrigger = builder.StartAt(firstNewTriggerTime)
                .WithSimpleSchedule(s => s.WithInterval(newInterval).RepeatForever())
                .Build();
            context.Scheduler.RescheduleJob(oldTrigger.Key, newTrigger);
        }

        private static bool DatesAreClose(DateTimeOffset original, DateTimeOffset toCompare,
            TimeSpan errorMargin = default)
        {
            if (errorMargin == default)
                errorMargin = TimeSpan.FromSeconds(5);

            var differenceInSeconds = Math.Abs(original.Subtract(toCompare).Seconds);
            return +differenceInSeconds < errorMargin.Seconds;
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