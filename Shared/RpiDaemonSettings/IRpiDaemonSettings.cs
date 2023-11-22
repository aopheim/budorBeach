namespace Shared.RpiDaemonSettings;

public interface IRpiDaemonSettings
{
    public bool TakeImagesInTheDark { get; set; }
    public int PictureIntervalInMinutes { get; set; }
    public double SpeciesRecognitionConfidence { get; set; }
}

public class RpiDaemonSettings : IRpiDaemonSettings
{
    public RpiDaemonSettings(bool takeImagesInTheDark = false, int pictureIntervalInMinutes = 60,
        double speciesRecognitionConfidence = 0.7)
    {
        TakeImagesInTheDark = takeImagesInTheDark;
        PictureIntervalInMinutes = pictureIntervalInMinutes;
        SpeciesRecognitionConfidence = speciesRecognitionConfidence;
    }

    public bool TakeImagesInTheDark { get; set; }
    public int PictureIntervalInMinutes { get; set; }
    public double SpeciesRecognitionConfidence { get; set; }
}