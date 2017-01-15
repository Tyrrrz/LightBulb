using System;
using System.Runtime.Serialization;
using LightBulb.Models;
using NegativeLayer.Settings;

namespace LightBulb
{
    public class Settings : SettingsManager
    {
        public static Settings Default { get; } = new Settings();

        private bool _isGammaPollingEnabled = true;
        private bool _isTemperatureSmoothingEnabled = true;
        private bool _disableWhenFullscreen;
        private bool _isInternetTimeSyncEnabled = true;
        private ushort _temperatureEpsilon = 50;
        private ushort _defaultMonitorTemperature = 6600;
        private ushort _minimumSmoothingTemperature = 1000;
        private ushort _maxTemperature = 6600;
        private ushort _minTemperature = 3900;
        private TimeSpan _temperatureSmoothingDuration = TimeSpan.FromSeconds(2);
        private TimeSpan _temperatureSwitchDuration = TimeSpan.FromMinutes(90);
        private TimeSpan _temperatureUpdateInterval = TimeSpan.FromMinutes(1);
        private TimeSpan _gammaPollingInterval = TimeSpan.FromSeconds(5);
        private TimeSpan _internetSyncInterval = TimeSpan.FromHours(6);
        private TimeSpan _sunriseTime = new TimeSpan(7, 20, 0);
        private TimeSpan _sunsetTime = new TimeSpan(16, 30, 0);
        private GeolocationInfo _geoInfo;

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

        public bool DisableWhenFullscreen
        {
            get { return _disableWhenFullscreen; }
            set { Set(ref _disableWhenFullscreen, value); }
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

        public ushort MinimumSmoothingTemperature
        {
            get { return _minimumSmoothingTemperature; }
            set { Set(ref _minimumSmoothingTemperature, value); }
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

        public TimeSpan TemperatureSmoothingDuration
        {
            get { return _temperatureSmoothingDuration; }
            set { Set(ref _temperatureSmoothingDuration, value); }
        }

        public TimeSpan TemperatureSwitchDuration
        {
            get { return _temperatureSwitchDuration; }
            set
            {
                if (Set(ref _temperatureSwitchDuration, value)) return;

                RaisePropertyChanged(() => TemperatureSwitchDurationMinutes);
            }
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

                RaisePropertyChanged(() => SunriseTimeHours);

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

                RaisePropertyChanged(() => SunsetTimeHours);

                if (SunsetTime < SunriseTime)
                    SunriseTime = SunsetTime;
            }
        }

        public GeolocationInfo GeoInfo
        {
            get { return _geoInfo; }
            set
            {
                if (Set(ref _geoInfo, value)) return;

                RaisePropertyChanged(() => GeoinfoNotNull);
            }
        }

        [IgnoreDataMember]
        public double TemperatureSwitchDurationMinutes
        {
            get { return TemperatureSwitchDuration.TotalMinutes; }
            set { TemperatureSwitchDuration = TimeSpan.FromMinutes(value); }
        }

        [IgnoreDataMember]
        public double SunriseTimeHours
        {
            get { return SunriseTime.TotalHours; }
            set { SunriseTime = TimeSpan.FromHours(value); }
        }

        [IgnoreDataMember]
        public double SunsetTimeHours
        {
            get { return SunsetTime.TotalHours; }
            set { SunsetTime = TimeSpan.FromHours(value); }
        }

        [IgnoreDataMember]
        public bool GeoinfoNotNull => GeoInfo != null;
    }
}