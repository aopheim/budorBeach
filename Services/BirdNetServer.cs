using System;
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
        private static readonly HttpClient Client = new HttpClient();

        public Task<bool> StartBirdNetServer()
        {
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = @"/C cd C:\repos\BirdNET-Analyzer && py server.py";
            process.StartInfo = startInfo;

            return Task.FromResult(process.Start());
        }

        public async Task<string> PostAsync(FileStream audioFileAsStream, CancellationToken cancellationToken)
        {
            var form = GetMultipartForm(audioFileAsStream);
            var responseMessage =
                await Client.PostAsync(GlobalConstants.BirdNetServerUrl, form, cancellationToken);
            return await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        }

        private MultipartFormDataContent GetMultipartForm(FileStream audioFileAsStream)
        {
            var form = new MultipartFormDataContent();
            var dto = new RecordingAnalyzerInputDto
            {
                Lat = GlobalConstants.BudorLatitude,
                Long = GlobalConstants.BudorLongitude,
                Week = ISOWeek.GetWeekOfYear(DateTime.UtcNow),
            };
            form.Add(new StringContent(JsonSerializer.Serialize(dto)), "meta");
            form.Add(new StreamContent(audioFileAsStream), "audio", "recording.wav");

            return form;
        }
    }
}