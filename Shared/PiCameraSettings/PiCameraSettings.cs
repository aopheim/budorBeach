namespace Shared.PiCameraSettings
{
    public class PiCameraSettings
    {
        public PiCameraSettings(int iso = 200, int shutterTime = 10)
        {
            Iso = iso;
            ShutterTime = shutterTime;
        }

        public PiCameraSettings() : this(default, default)
        {
        }

        public int Iso { get; set; }
        public int ShutterTime { get; set; }
    }
}