using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dtos;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Shared;

namespace Services
{
    public class BirdNetServer : IBirdNetServer
    {
        private static readonly HttpClient Client = new();
        private readonly ILogger _logger;

        public BirdNetServer(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> PostAsync(string filePath, CancellationToken cancellationToken)
        {
            await using var audioFileAsStream = new FileStream(filePath, FileMode.Open,
                FileAccess.Read);
            var form = GetMultipartForm(audioFileAsStream);
            var birdNetServerUrl = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? GlobalConstants.BirdNetServerWindowsUrl
                : GlobalConstants.BirdNetServerDockerUrl;
            _logger.LogInformation($"Sending recording on path {filePath} to BirdNetServer on {birdNetServerUrl}");
            var responseMessage =
                await Client.PostAsync(birdNetServerUrl, form, cancellationToken);
            return await responseMessage.Content.ReadAsStringAsync(cancellationToken);
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