namespace Shared.PiCameraSettings
{
    public class PiCameraSettings
    {
        private const int DefaultIso = 200;
        private const int DefaultShutterTime = 10;

        public PiCameraSettings(int iso = DefaultIso, int shutterTime = DefaultShutterTime)
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