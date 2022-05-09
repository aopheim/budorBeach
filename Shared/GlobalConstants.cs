using System;

namespace Shared
{
    public static class GlobalConstants
    {
        public const string ImagesContainerName = "images";
        public const string ThumbnailImagesContainerName = "images-thumbnails";
        public const string AudioRecordingsContainerName = "audio-recordings";

        public const string DevelopmentUrl = "http://localhost:3000";
        public const string ProductionUrl = "https://budorbeach.no";
        public const string HubEndpoint = "/budorhub";
        public const string DevelopmentHubUrl = DevelopmentUrl + HubEndpoint;
        public const string ProductionHubUrl = ProductionUrl + HubEndpoint;
        public const string BirdNetServerUrl = "http://localhost:8080/analyze";

        public const string SecondJobs = "secondJobs";
        public const string DailyJobs = "dailyJobs";

        public const string PictureTrigger = "pictureTrigger";
        public const string ProximityTrigger = "proximityTrigger";
        public const string Bme280Trigger = "bme280Trigger";
        public const string AudioRecordingTrigger = "audioRecordingTrigger";
        public const string AudioAnalyzerTrigger = "audioAnalyzerTrigger";
        public const string VideoRecordingTrigger = "videoRecordingTrigger";

        public const double BudorLatitude = 60.974951;
        public const double BudorLongitude = 11.285140;

        public const string AudioRecordingsFolderLinux = @"home\pi\audioRecordings\";
        public const string AudioServiceFolderLinux = @"home\pi\budorBeach\AudioService";
        public const string AudioServiceFolderWindows = @"C:\repos\budorBeach\AudioService";
    }
}