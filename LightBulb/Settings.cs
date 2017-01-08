using System;
using NegativeLayer.Settings;

namespace LightBulb
{
    public class Settings : SettingsManager
    {
        public static Settings Default { get; } = new Settings();

        private ushort _temperatureEpsilon = 50;
        public ushort TemperatureEpsilon
        {
            get { return _temperatureEpsilon; }
            set { Set(ref _temperatureEpsilon, value); }
        }

        private ushort _minTemperature = 3900;
        public ushort MinTemperature
        {
            get { return _minTemperature; }
            set { Set(ref _minTemperature, value); }
        }

        private ushort _maxTemperature = 6500;
        public ushort MaxTemperature
        {
            get { return _maxTemperature; }
            set { Set(ref _maxTemperature, value); }
        }

        private TimeSpan _temperatureSwitchOffset = TimeSpan.FromMinutes(90);
        public TimeSpan TemperatureSwitchOffset
        {
            get { return _temperatureSwitchOffset; }
            set { Set(ref _temperatureSwitchOffset, value); }
        }

        private TimeSpan _temperatureUpdateInterval = TimeSpan.FromMinutes(1);
        public TimeSpan TemperatureUpdateInterval
        {
            get { return _temperatureUpdateInterval; }
            set { Set(ref _temperatureUpdateInterval, value); }
        }

        private bool _isPollingEnabled = true;
        public bool IsPollingEnabled
        {
            get { return _isPollingEnabled; }
            set { Set(ref _isPollingEnabled, value); }
        }

        private TimeSpan _pollingInterval = TimeSpan.FromSeconds(5);
        public TimeSpan PollingInterval
        {
            get { return _pollingInterval; }
            set { Set(ref _pollingInterval, value); }
        }

        private TimeSpan _sunriseTime = new TimeSpan(7, 20, 0);
        public TimeSpan SunriseTime
        {
            get { return _sunriseTime; }
            set { Set(ref _sunriseTime, value); }
        }

        private TimeSpan _sunsetTime = new TimeSpan(16, 30, 0);
        public TimeSpan SunsetTime
        {
            get { return _sunsetTime; }
            set { Set(ref _sunsetTime, value); }
        }

        private bool _disableWhenFullscreen;
        public bool DisableWhenFullscreen
        {
            get { return _disableWhenFullscreen; }
            set { Set(ref _disableWhenFullscreen, value); }
        }
    }
}