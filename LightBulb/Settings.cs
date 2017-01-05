using System;
using NegativeLayer.Settings;

namespace LightBulb
{
    public class Settings : SettingsManager
    {
        public static Settings Default { get; } = new Settings();

        public ushort TemperatureEpsilon { get; set; } = 50;
        public ushort MinTemperature { get; set; } = 3900;
        public ushort MaxTemperature { get; set; } = 6500;
        public bool IsPollingEnabled { get; set; } = true;
        public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMinutes(5);
        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);
        public bool DisableWhenFullscreen { get; set; } = false;
    }
}
