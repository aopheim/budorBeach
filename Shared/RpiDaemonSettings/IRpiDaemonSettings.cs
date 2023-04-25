namespace Shared.RpiDaemonSettings;

public interface IRpiDaemonSettings
{
    public int PictureIntervalInMinutes { get; set; }
    public double SpeciesRecognitionConfidence { get; set; }
}

public class RpiDaemonSettings : IRpiDaemonSettings
{
    public int PictureIntervalInMinutes { get; set; }
    public double SpeciesRecognitionConfidence { get; set; }
}