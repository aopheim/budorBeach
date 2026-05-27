using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using CameraService.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.PiCameraSettings;
using Microsoft.Extensions.Http;

namespace CameraService
{
    public partial class CameraService : ICameraService
    {
        private readonly bool _isWindows;
        private readonly ILogger<CameraService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private bool _cameraIsInUse;

        public CameraService(ILogger<CameraService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            _cameraIsInUse = false;
            _httpClientFactory = httpClientFactory;
        }

        private string GetCameraServiceUrl()
        {
            var url = Environment.GetEnvironmentVariable("CAMERA_SERVICE_URL") ?? "http://localhost:8000";
            _logger.LogInformation($"Camera Service URL: {url}");
            return url;
        }

        public bool CameraIsInUse()
        {
            return _cameraIsInUse;
        }

        public async Task TakeImage(string fullPath, PiCameraSettings settings)
        {
            _cameraIsInUse = true;
            try
            {
                if (_isWindows)
                {
                    _logger.LogInformation("Running on Windows. Mocking image capture...");
                    return;
                }

                _logger.LogInformation($"Taking image: {fullPath} (ISO: {settings.Iso}, Shutter: {settings.ShutterTime}ms)");

                var request = new
                {
                    filename = fullPath,
                    iso = settings.Iso,
                    shutter_speed = settings.ShutterTime
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                httpClient.BaseAddress = new Uri(GetCameraServiceUrl());
                
                var response = await httpClient.PostAsync("/takeimage", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Camera service returned error: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Camera service error: {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Image captured successfully: {responseBody}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error taking image via camera service");
                _cameraIsInUse = false;
                throw;
            }

            _cameraIsInUse = false;
        }

        public Task CaptureVideo(string fullPath, int secondsToRecord)
        {
            throw new NotImplementedException();
        }
    }
}