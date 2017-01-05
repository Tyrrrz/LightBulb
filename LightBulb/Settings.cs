using NegativeLayer.Settings;

namespace LightBulb
{
    public class Settings : SettingsManager
    {
        public static Settings Default { get; } = new Settings();

        public ushort SmallestTemperatureInterval { get; set; } = 50;
        public ushort MinTemperature { get; set; } = 3900;
        public ushort MaxTemperature { get; set; } = 6500;
        public bool IsPollingEnabled { get; set; } = true;
        public double PollingInterval { get; set; } = 5000;
        public bool DisableWhenFullscreen { get; set; } = false;
    }
}
