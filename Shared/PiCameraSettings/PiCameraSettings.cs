namespace Shared.PiCameraSettings
{
    public class PiCameraSettings
    {
        public const int DefaultIso = 800;
        public const int DefaultShutterTimeInMs = 0;

        public PiCameraSettings(int iso = DefaultIso, int shutterTime = DefaultShutterTimeInMs)
        {
            Iso = iso;
            ShutterTime = shutterTime;
        }

        public PiCameraSettings() : this(DefaultIso)
        {
        }

        public int Iso { get; set; }
        public int ShutterTime { get; set; }
    }
}