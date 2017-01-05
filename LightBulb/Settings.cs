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

        private bool _isPollingEnabled = true;
        public bool IsPollingEnabled
        {
            get { return _isPollingEnabled; }
            set { Set(ref _isPollingEnabled, value); }
        }

        private TimeSpan _updateInterval = TimeSpan.FromMinutes(5);
        public TimeSpan UpdateInterval
        {
            get { return _updateInterval; }
            set { Set(ref _updateInterval, value); }
        }

        private TimeSpan _pollingInterval = TimeSpan.FromSeconds(5);
        public TimeSpan PollingInterval
        {
            get { return _pollingInterval; }
            set { Set(ref _pollingInterval, value); }
        }

        private bool _disableWhenFullscreen;
        public bool DisableWhenFullscreen
        {
            get { return _disableWhenFullscreen; }
            set { Set(ref _disableWhenFullscreen, value); }
        }
    }
}