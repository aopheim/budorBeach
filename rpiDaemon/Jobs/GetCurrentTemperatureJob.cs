using System;
using System.IO.Ports;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using rpiDaemon.Models;

namespace rpiDaemon.Jobs
{
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

            await _context.SensorReadings.AddAsync(sensorReadingModel);

            await _context.SaveChangesAsync(context.CancellationToken);
        }

        private SensorReadingModel GetCurrentSensorReadings()
        {
            const string portName = "COM5";
            var sensorModel = new SensorReadingModel();

            using var port = new SerialPort(portName)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None
            };
            port.DataReceived += SerialPortDataReceived;

            void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                var serialPort = (SerialPort) sender;

                var serialData = serialPort.ReadExisting();

                sensorModel = JsonConvert.DeserializeObject<SensorReadingModel>(serialData);
                sensorModel.MeasuredAtUtc = DateTime.UtcNow;
            }

            _logger.LogInformation($"{sensorModel.TemperatureInDegreesC}");
            port.Open();
            port.Close();
            port.Dispose();
            return sensorModel;
        }
    }
}