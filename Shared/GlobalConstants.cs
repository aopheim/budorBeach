namespace Shared;

public static class GlobalConstants
{
    public const string AppInsightsConnectionString = "AppInsightsConnectionString";
    public const string ImagesContainerName = "images";
    public const string ThumbnailImagesContainerName = "images-thumbnails";
    public const string AudioRecordingsContainerName = "audio-recordings";

    public const string DevelopmentUrl = "http://localhost:3000";
    public const string ProductionUrl = "https://budorbeach.no";
    public const string HubEndpoint = "/budorhub";
    public const string DevelopmentHubUrl = DevelopmentUrl + HubEndpoint;
    public const string ProductionHubUrl = ProductionUrl + HubEndpoint;
    public const string BirdNetServerWindowsUrl = "http://localhost:8080/analyze";
    public const string BirdNetServerDockerUrl = "http://birdnetserver:8080/analyze";
    public const string AudioRecorderWindowsUrl = "http://audiorecorderwindows:4000/record";
    public const string AudioRecorderLinuxUrl = "http://audiorecorderlinux:4000/record";

    public const string SecondJobs = "secondJobs";
    public const string MinuteJobs = "minuteJobs";
    public const string DailyJobs = "dailyJobs";

    public const string PictureTrigger = "pictureTrigger";
    public const string ProximityTrigger = "proximityTrigger";
    public const string Bme280Trigger = "bme280Trigger";
    public const string AudioRecordingTrigger = "audioRecordingTrigger";
    public const string AudioAnalyzerTrigger = "audioAnalyzerTrigger";
    public const string VideoRecordingTrigger = "videoRecordingTrigger";
    public const string UploadAudioRecordingTrigger = "uploadAudioRecordingTrigger";
    public const string StartVideSurveillanceTrigger = "startVideoSurveillanceTrigger";
    public const string StartVideoStreamTrigger = "startVideoStreamTrigger";

    public const double BudorLatitude = 60.974951;
    public const double BudorLongitude = 11.285140;

    public const string AudioRecordingsFolderLinux = @"/audioRecordings/";
    public const string ImagesFolder = @"/images/";
    public const string AudioRecordingsFolderWindows = @"%APPDATA%/budorBeach/audioRecordings";
    public const string VideoRecordingsFolderLinux = @"/home/pi/videos/";
    public const string AudioServiceFolderLinux = @"/home/pi/budorBeach/AudioService/";
    public const string BirdNetAnalyzerPathLinux = @"/home/pi/BirdNET-Analyzer/";
    public const string AudioServiceFolderWindows = @"C:\repos\budorBeach\AudioService";
    public const string BirdNetAnalyzerPathWindows = @"C:\repos\BirdNET-Analyzer";
    public const string ProductionDb = "ProductionDb";
    public const string DevelopmentDb = "DevelopmentDb";
    public const string DockerDevelopmentDb = "DockerDevelopmentDb";
}