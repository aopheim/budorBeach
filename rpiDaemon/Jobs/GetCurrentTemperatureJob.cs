using System;
using System.IO.Ports;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Quartz;
using rpiDaemon.Models;

namespace rpiDaemon.Jobs
{
    [UsedImplicitly]
    public class GetCurrentTemperatureJob : IJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetCurrentTemperatureJob> _logger;

        public GetCurrentTemperatureJob(ApplicationDbContext context, ILogger<GetCurrentTemperatureJob> logger)
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
            const string portName = "COM5";
            SensorReadingModel sensorModel = null;

            var port = new SerialPort(portName)
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
                Console.WriteLine(e);
                throw;
            }

            port.Close();
            port.Dispose();

            _logger.LogInformation($"{JsonSerializer.Serialize(sensorModel)}");
            return sensorModel;
        }
    }
}