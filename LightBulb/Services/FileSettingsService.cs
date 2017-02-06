using System;
using LightBulb.Models;
using LightBulb.Services.Interfaces;
using Tyrrrz.Settings;

namespace LightBulb.Services
{
    public sealed class FileSettingsService : SettingsManager, ISettingsService, IDisposable
    {
        private bool _isGammaPollingEnabled = true;
        private bool _isTemperatureSmoothingEnabled = true;
        private bool _isFullscreenBlocking;
        private bool _isInternetTimeSyncEnabled = true;
        private bool _shouldUpdateGeoInfo = true;
        private ushort _temperatureEpsilon = 50;
        private ushort _defaultMonitorTemperature = 6600;
        private ushort _maxTemperature = 6600;
        private ushort _minTemperature = 3900;
        private TimeSpan _maximumTemperatureSmoothingDuration = TimeSpan.FromSeconds(3);
        private TimeSpan _temperatureTransitionDuration = TimeSpan.FromMinutes(90);
        private TimeSpan _temperatureUpdateInterval = TimeSpan.FromMinutes(1);
        private TimeSpan _gammaPollingInterval = TimeSpan.FromSeconds(5);
        private TimeSpan _internetSyncInterval = TimeSpan.FromHours(6);
        private TimeSpan _sunriseTime = new TimeSpan(7, 20, 0);
        private TimeSpan _sunsetTime = new TimeSpan(16, 30, 0);
        private GeoInfo _geoInfo;
        private Hotkey _toggleHotkey;
        private Hotkey _togglePollingHotkey;
        private Hotkey _refreshGammaHotkey;

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

        public bool ShouldUpdateGeoInfo
        {
            get { return _shouldUpdateGeoInfo; }
            set { Set(ref _shouldUpdateGeoInfo, value); }
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
                if (!Set(ref _maxTemperature, value)) return;

                if (MaxTemperature < MinTemperature)
                    MinTemperature = MaxTemperature;
            }
        }

        public TimeSpan MaximumTemperatureSmoothingDuration
        {
            get { return _maximumTemperatureSmoothingDuration; }
            set { Set(ref _maximumTemperatureSmoothingDuration, value); }
        }

        public TimeSpan TemperatureTransitionDuration
        {
            get { return _temperatureTransitionDuration; }
            set { Set(ref _temperatureTransitionDuration, value); }
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
                if (!Set(ref _sunriseTime, value)) return;

                if (SunriseTime > SunsetTime)
                    SunsetTime = SunriseTime;
            }
        }

        public TimeSpan SunsetTime
        {
            get { return _sunsetTime; }
            set
            {
                if (!Set(ref _sunsetTime, value)) return;

                if (SunsetTime < SunriseTime)
                    SunriseTime = SunsetTime;
            }
        }

        public GeoInfo GeoInfo
        {
            get { return _geoInfo; }
            set { Set(ref _geoInfo, value); }
        }

        public Hotkey ToggleHotkey
        {
            get { return _toggleHotkey; }
            set
            {
                // Make sure other hotkeys don't use the same keys
                if (value != null)
                {
                    if (TogglePollingHotkey == value) TogglePollingHotkey = null;
                    if (RefreshGammaHotkey == value) RefreshGammaHotkey = null;
                }

                Set(ref _toggleHotkey, value);
            }
        }

        public Hotkey TogglePollingHotkey
        {
            get { return _togglePollingHotkey; }
            set
            {
                // Make sure other hotkeys don't use the same keys
                if (value != null)
                {
                    if (ToggleHotkey == value) ToggleHotkey = null;
                    if (RefreshGammaHotkey == value) RefreshGammaHotkey = null;
                }

                Set(ref _togglePollingHotkey, value);
            }
        }

        public Hotkey RefreshGammaHotkey
        {
            get { return _refreshGammaHotkey; }
            set
            {
                // Make sure other hotkeys don't use the same keys
                if (value != null)
                {
                    if (ToggleHotkey == value) ToggleHotkey = null;
                    if (TogglePollingHotkey == value) TogglePollingHotkey = null;
                }

                Set(ref _refreshGammaHotkey, value);
            }
        }

        public FileSettingsService()
            : base(new Configuration {SubDirectoryPath = "LightBulb", FileName = "Configuration.dat"})
        {
            TryLoad();
        }

        public void Dispose()
        {
            TrySave();
        }
    }
}