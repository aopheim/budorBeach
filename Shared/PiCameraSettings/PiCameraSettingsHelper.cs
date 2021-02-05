using System;
using System.IO;
using System.Text.Json;

namespace Shared.PiCameraSettings
{
    public static class PiCameraSettingsHelper
    {
        private const string FileName = "currentPiCameraSettings.json";

        public static PiCameraSettings GetCurrentCameraSettingsFromFile()
        {
            try
            {
                var json = File.ReadAllText(FileName);
                return JsonSerializer.Deserialize<PiCameraSettings>(json);
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException)
                    return new PiCameraSettings();
                throw;
            }
        }

        public static void ResetCurrentCameraSettings()
        {
            SetCameraSettingsToFile(new PiCameraSettings());
        }

        public static void SetCameraSettingsToFile(PiCameraSettings updatedSettings)
        {
            var json = JsonSerializer.Serialize(updatedSettings);
            File.WriteAllText(FileName, json);
        }
    }
}