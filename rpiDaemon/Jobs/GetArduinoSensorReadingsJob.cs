using System;
using System.IO.Ports;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Quartz;
using Shared.Models;

namespace rpiDaemon.Jobs
{
    [UsedImplicitly]
    public class GetArduinoSensorReadingsJob : IJob
    {
        private const string PortName = "/dev/ttyUSB0";
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetArduinoSensorReadingsJob> _logger;

        public GetArduinoSensorReadingsJob(ApplicationDbContext context, ILogger<GetArduinoSensorReadingsJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var sensorReadingModel = GetCurrentSensorReadings();

            if (sensorReadingModel != null)
            {
                await _context.SensorReadings.AddAsync(sensorReadingModel);
                await _context.SaveChangesAsync(context.CancellationToken);
            }
        }

        [CanBeNull]
        private SensorReadingModel GetCurrentSensorReadings()
        {
            SensorReadingModel sensorModel = null;

            var port = new SerialPort(PortName)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None
            };

            try
            {
                if (!port.IsOpen)
                {
                    port.Open();

                    var serialData = port.ReadLine();
                    sensorModel = JsonSerializer.Deserialize<SensorReadingModel>(serialData);
                    sensorModel.MeasuredAtUtc = DateTime.UtcNow;
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentException) _logger.LogError("Argument exception. Probably usb port not found");
                else
                    throw;
            }

            port.Close();
            port.Dispose();

            _logger.LogInformation($"{JsonSerializer.Serialize(sensorModel)}");
            return sensorModel;
        }
    }
}