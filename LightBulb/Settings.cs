using System;
using LightBulb.Models;
using Tyrrrz.Settings;

namespace LightBulb
{
    public class Settings : SettingsManager
    {
        public static Settings Default { get; } = new Settings();

        private bool _isGammaPollingEnabled;
        private bool _isTemperatureSmoothingEnabled = true;
        private bool _isFullscreenBlocking;
        private bool _isInternetTimeSyncEnabled = true;
        private ushort _temperatureEpsilon = 50;
        private ushort _defaultMonitorTemperature = 6600;
        private ushort _maxTemperature = 6600;
        private ushort _minTemperature = 3900;
        private TimeSpan _maximumTemperatureSmoothingDuration = TimeSpan.FromSeconds(3);
        private TimeSpan _temperatureSwitchDuration = TimeSpan.FromMinutes(90);
        private TimeSpan _temperatureUpdateInterval = TimeSpan.FromMinutes(1);
        private TimeSpan _gammaPollingInterval = TimeSpan.FromSeconds(5);
        private TimeSpan _internetSyncInterval = TimeSpan.FromHours(6);
        private TimeSpan _sunriseTime = new TimeSpan(7, 20, 0);
        private TimeSpan _sunsetTime = new TimeSpan(16, 30, 0);
        private GeoInfo _geoInfo;

        public bool IsGammaPollingEnabled
        {
            get { return _isGammaPollingEnabled; }
            set { Set(ref _isGammaPollingEnabled, value); }
        }

        public bool IsTemperatureSmoothingEnabled
        {
            get { return _isTemperatureSmoothingEnabled; }
            set { Set(ref _isTemperatureSmoothingEnabled, value); }
        }

        public bool IsFullscreenBlocking
        {
            get { return _isFullscreenBlocking; }
            set { Set(ref _isFullscreenBlocking, value); }
        }

        public bool IsInternetTimeSyncEnabled
        {
            get { return _isInternetTimeSyncEnabled; }
            set { Set(ref _isInternetTimeSyncEnabled, value); }
        }

        public ushort TemperatureEpsilon
        {
            get { return _temperatureEpsilon; }
            set { Set(ref _temperatureEpsilon, value); }
        }

        public ushort DefaultMonitorTemperature
        {
            get { return _defaultMonitorTemperature; }
            set { Set(ref _defaultMonitorTemperature, value); }
        }

        public ushort MinTemperature
        {
            get { return _minTemperature; }
            set
            {
                if (!Set(ref _minTemperature, value)) return;

                if (MinTemperature > MaxTemperature)
                    MaxTemperature = MinTemperature;
            }
        }

        public ushort MaxTemperature
        {
            get { return _maxTemperature; }
            set
            {
                if (Set(ref _maxTemperature, value)) return;

                if (MaxTemperature < MinTemperature)
                    MinTemperature = MaxTemperature;
            }
        }

        public TimeSpan MaximumTemperatureSmoothingDuration
        {
            get { return _maximumTemperatureSmoothingDuration; }
            set { Set(ref _maximumTemperatureSmoothingDuration, value); }
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

        public TimeSpan InternetSyncInterval
        {
            get { return _internetSyncInterval; }
            set { Set(ref _internetSyncInterval, value); }
        }

        public TimeSpan SunriseTime
        {
            get { return _sunriseTime; }
            set
            {
                if (Set(ref _sunriseTime, value)) return;

                if (SunriseTime > SunsetTime)
                    SunsetTime = SunriseTime;
            }
        }

        public TimeSpan SunsetTime
        {
            get { return _sunsetTime; }
            set
            {
                if (Set(ref _sunsetTime, value)) return;

                if (SunsetTime < SunriseTime)
                    SunriseTime = SunsetTime;
            }
        }

        public GeoInfo GeoInfo
        {
            get { return _geoInfo; }
            set { Set(ref _geoInfo, value); }
        }
    }
}