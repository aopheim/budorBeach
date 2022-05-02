using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.Interfaces;
using Shared;

namespace rpiDaemon.Jobs
{
    public class AnalyzeAudioRecordingsJob : IJob
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AnalyzeAudioRecordingsJob> _logger;
        private readonly IBirdNetResultConverter _resultConverter;

        public AnalyzeAudioRecordingsJob(ILogger<AnalyzeAudioRecordingsJob> logger, IWebHostEnvironment environment,
            IBirdNetResultConverter resultConverter)
        {
            _logger = logger;
            _environment = environment;
            _resultConverter = resultConverter;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var url = GlobalConstants.BirdNetServerUrl;
            var recordingsFolderName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? @"C:\Users\AdrianOpheim\Documents\budorBeach\AudioService"
                : @"home\pi\audioRecordings";
            var fullFileNames = Directory.GetFiles(recordingsFolderName);
            var recordingFileNames = fullFileNames.Select(Path.GetFileNameWithoutExtension).ToList();
            

            await using var audioFileAsStream =
                new FileStream(recordingsFolderName, FileMode.Open,
                    FileAccess.Read);
            var form = new MultipartFormDataContent();
            var dto = new RecordingAnalyzerInputDto
            {
                Lat = GlobalConstants.BudorLatitude,
                Long = GlobalConstants.BudorLongitude,
                Week = ISOWeek.GetWeekOfYear(DateTime.UtcNow),
            };
            form.Add(new StringContent(JsonSerializer.Serialize(dto)), "meta");
            form.Add(new StreamContent(audioFileAsStream), "audio", "kjottmeis.wav");

            var res = await Client.PostAsync(url, form, context.CancellationToken);
            var response = await res.Content.ReadAsStringAsync(context.CancellationToken);
            _logger.LogInformation($"Response from server: {response}");
            var result = _resultConverter.ConvertJson(response);
            _logger.LogInformation(result.Message);
        }
    }
}