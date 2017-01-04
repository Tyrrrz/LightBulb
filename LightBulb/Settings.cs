using NegativeLayer.Settings;

namespace LightBulb
{
    public class Settings : SettingsManager
    {
        public static Settings Default { get; } = new Settings();

        public ushort MinTemperature { get; set; } = 3900;
        public ushort MaxTemperature { get; set; } = 6500;
    }
}
