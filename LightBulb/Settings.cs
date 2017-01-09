using System;
using System.Runtime.Serialization;
using NegativeLayer.Settings;

namespace LightBulb
{
    public class Settings : SettingsManager
    {
        public static Settings Default { get; } = new Settings();

        private bool _isGammaPollingEnabled = true;
        private bool _disableWhenFullscreen;
        private ushort _temperatureEpsilon = 50;
        private ushort _maxTemperature = 6500;
        private ushort _minTemperature = 3900;
        private TimeSpan _temperatureSwitchDuration = TimeSpan.FromMinutes(90);
        private TimeSpan _temperatureUpdateInterval = TimeSpan.FromMinutes(1);
        private TimeSpan _gammaPollingInterval = TimeSpan.FromSeconds(5);
        private TimeSpan _sunriseTime = new TimeSpan(7, 20, 0);
        private TimeSpan _sunsetTime = new TimeSpan(16, 30, 0);

        public bool IsGammaPollingEnabled
        {
            get { return _isGammaPollingEnabled; }
            set { Set(ref _isGammaPollingEnabled, value); }
        }

        public bool DisableWhenFullscreen
        {
            get { return _disableWhenFullscreen; }
            set { Set(ref _disableWhenFullscreen, value); }
        }

        public ushort TemperatureEpsilon
        {
            get { return _temperatureEpsilon; }
            set { Set(ref _temperatureEpsilon, value); }
        }

        public ushort MinTemperature
        {
            get { return _minTemperature; }
            set { Set(ref _minTemperature, value); }
        }

        public ushort MaxTemperature
        {
            get { return _maxTemperature; }
            set { Set(ref _maxTemperature, value); }
        }

        public TimeSpan TemperatureSwitchDuration
        {
            get { return _temperatureSwitchDuration; }
            set { Set(ref _temperatureSwitchDuration, value); }
        }

        public TimeSpan TemperatureUpdateInterval
        {
            get { return _temperatureUpdateInterval; }
            set { Set(ref _temperatureUpdateInterval, value); }
        }

        public TimeSpan GammaPollingInterval
        {
            get { return _gammaPollingInterval; }
            set { Set(ref _gammaPollingInterval, value); }
        }

        public TimeSpan SunriseTime
        {
            get { return _sunriseTime; }
            set { Set(ref _sunriseTime, value); }
        }

        public TimeSpan SunsetTime
        {
            get { return _sunsetTime; }
            set { Set(ref _sunsetTime, value); }
        }

        [IgnoreDataMember]
        public double TemperatureSwitchDurationMinutes
        {
            get { return TemperatureSwitchDuration.TotalMinutes; }
            set { TemperatureSwitchDuration = TimeSpan.FromMinutes(value); }
        }
    }
}