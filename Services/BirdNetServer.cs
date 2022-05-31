using System;
using System.Diagnostics;
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
        private Process _process;

        public BirdNetServer(ILogger logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting up BirdNet server...");
            _process = new Process();
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var birdNetAnalyzerPath = isWindows
                ? GlobalConstants.BirdNetAnalyzerPathWindows
                : GlobalConstants.BirdNetAnalyzerPathLinux;
            var pythonAbbr = isWindows ? "py" : "python";
            var windowsArguments = @$"/C cd {birdNetAnalyzerPath} && {pythonAbbr} server.py";
            var linuxArguments = @$"cd {birdNetAnalyzerPath} && {pythonAbbr} server.py";

            var startInfoWindows = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = windowsArguments
            };
            var startInfoLinux = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "/bin/bash",
                Arguments = linuxArguments,
                RedirectStandardOutput = true
            };
            _process.StartInfo = isWindows ? startInfoWindows : startInfoLinux;

            return Task.FromResult(_process.Start());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shutting down BirdNetServer...");
            return Task.FromResult(_process.WaitForExit(1));
        }

        public async Task<string> PostAsync(string filePath, CancellationToken cancellationToken)
        {
            await using var audioFileAsStream = new FileStream(filePath, FileMode.Open,
                FileAccess.Read);
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
                Week = ISOWeek.GetWeekOfYear(DateTime.UtcNow)
            };
            form.Add(new StringContent(JsonSerializer.Serialize(dto)), "meta");
            form.Add(new StreamContent(audioFileAsStream), "audio", "recording.wav");

            return form;
        }
    }
}