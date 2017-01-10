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
        private bool _isInternetTimeSyncEnabled;
        private ushort _temperatureEpsilon = 50;
        private ushort _maxTemperature = 6500;
        private ushort _minTemperature = 3900;
        private TimeSpan _temperatureSwitchDuration = TimeSpan.FromMinutes(90);
        private TimeSpan _temperatureUpdateInterval = TimeSpan.FromMinutes(1);
        private TimeSpan _gammaPollingInterval = TimeSpan.FromSeconds(3);
        private TimeSpan _internetSyncInterval = TimeSpan.FromHours(6);
        private TimeSpan _sunriseTime = new TimeSpan(7, 20, 0);
        private TimeSpan _sunsetTime = new TimeSpan(16, 30, 0);
        private double _longitude = double.NaN;
        private double _latitude = double.NaN;

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

        public ushort MinTemperature
        {
            get { return _minTemperature; }
            set
            {
                Set(ref _minTemperature, value);

                if (MinTemperature > MaxTemperature)
                    MaxTemperature = MinTemperature;
            }
        }

        public ushort MaxTemperature
        {
            get { return _maxTemperature; }
            set
            {
                Set(ref _maxTemperature, value);

                if (MaxTemperature < MinTemperature)
                    MinTemperature = MaxTemperature;
            }
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
                Set(ref _sunriseTime, value);

                if (SunriseTime > SunsetTime)
                    SunsetTime = SunriseTime;
            }
        }

        public TimeSpan SunsetTime
        {
            get { return _sunsetTime; }
            set
            {
                Set(ref _sunsetTime, value);

                if (SunsetTime < SunriseTime)
                    SunriseTime = SunsetTime;
            }
        }

        public double Latitude
        {
            get { return _latitude; }
            set { Set(ref _latitude, value); }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { Set(ref _longitude, value); }
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
    }
}