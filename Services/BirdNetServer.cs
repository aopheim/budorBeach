using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dtos;
using Services.Interfaces;
using Shared;

namespace Services
{
    public class BirdNetServer : IBirdNetServer
    {
        private static readonly HttpClient Client = new();
        private Process _process;

        public async Task<string> PostAsync(FileStream audioFileAsStream, CancellationToken cancellationToken)
        {
            var form = GetMultipartForm(audioFileAsStream);
            var responseMessage =
                await Client.PostAsync(GlobalConstants.BirdNetServerUrl, form, cancellationToken);
            return await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _process = new Process();
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = @"/C cd C:\repos\BirdNET-Analyzer && py server.py"
            };
            _process.StartInfo = startInfo;

            return Task.FromResult(_process.Start());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_process.WaitForExit(1));
        }

        private MultipartFormDataContent GetMultipartForm(FileStream audioFileAsStream)
        {
            var form = new MultipartFormDataContent();
            var dto = new RecordingAnalyzerInputDto
            {
                Lat = GlobalConstants.BudorLatitude,
                Long = GlobalConstants.BudorLongitude,
                Week = ISOWeek.GetWeekOfYear(DateTime.UtcNow)
            };
            form.Add(new StringContent(JsonSerializer.Serialize(dto)), "meta");
            form.Add(new StreamContent(audioFileAsStream), "audio", "recording.wav");

            return form;
        }
    }
}