namespace Shared
{
    public static class GlobalConstants
    {
        public const string AppInsightsConnectionString = "AppInsightsConnectionString";
        public const string ImagesContainerName = "images";
        public const string ThumbnailImagesContainerName = "images-thumbnails";
        public const string DevelopmentUrl = "http://localhost:3000";
        public const string ProductionUrl = "https://budorbeach.no";
        public const string HubEndpoint = "/budorhub";
        public const string DevelopmentHubUrl = DevelopmentUrl + HubEndpoint;
        public const string ProductionHubUrl = ProductionUrl + HubEndpoint;
        public const string ProductionDb = "ProductionDb";
        public const string DevelopmentDb = "DevelopmentDb";
    }
}