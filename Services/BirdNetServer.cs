using System;
using System.Globalization;
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
            var form = GetMultipartForm(filePath);
            var birdNetServerUrl = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? GlobalConstants.BirdNetServerWindowsUrl
                : GlobalConstants.BirdNetServerDockerUrl;
            _logger.LogInformation($"Sending recording on path {filePath} to BirdNetServer on {birdNetServerUrl}");
            var responseMessage =
                await Client.PostAsync(birdNetServerUrl, form, cancellationToken);
            return await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        }

        private MultipartFormDataContent GetMultipartForm(string filePath)
        {
            var form = new MultipartFormDataContent();
            var dto = new RecordingAnalyzerInputDto
            {
                Lat = GlobalConstants.BudorLatitude,
                Lon = GlobalConstants.BudorLongitude,
                Week = ISOWeek.GetWeekOfYear(DateTime.UtcNow),
                FilePath = filePath
            };
            form.Add(new StringContent(JsonSerializer.Serialize(dto)), "meta");

            return form;
        }
    }
}