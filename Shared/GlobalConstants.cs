namespace Shared
{
    public static class GlobalConstants
    {
        public const string ImagesContainerName = "images";
        public const string ThumbnailImagesContainerName = "images-thumbnails";
        public const string DevelopmentUrl = "http://localhost:3000";
        public const string ProductionUrl = "https://budorbeach.no";
        public const string HubEndpoint = "/budorhub";
        public const string DevelopmentHubUrl = DevelopmentUrl + HubEndpoint;
        public const string ProductionHubUrl = ProductionUrl + HubEndpoint;
        public const string SecondJobs = "secondJobs";
        public const string DailyJobs = "dailyJobs";
        public const string PictureTrigger = "pictureTrigger";
        public const string Bme280Trigger = "bme280Trigger";
        public const string AudioRecordingTrigger = "audioRecordingTrigger";
        public const string AudioAnalyzerTrigger = "audioAnalyzerTrigger";
        public const string BirdNetServerUrl = "http://localhost:8080/analyze";
        public const double BudorLatitude = 60.974951;
        public const double BudorLongitude = 11.285140;
    }
}