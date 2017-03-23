using System;
using System.Net;
using LightBulb.Models;
using Tyrrrz.Extensions;
using Tyrrrz.Settings;

namespace LightBulb.Services
{
    public class FileSettingsService : SettingsManager, ISettingsService
    {
        private bool _isGammaPollingEnabled = true;
        private bool _isTemperatureSmoothingEnabled = true;
        private bool _isFullscreenBlocking;
        private bool _isCheckForUpdatedEnabled = true;
        private bool _isInternetSyncEnabled = true;
        private bool _isGeoInfoLocked;
        private ushort _temperatureEpsilon = 50;
        private ushort _defaultMonitorTemperature = 6600;
        private ushort _maxTemperature = 6600;
        private ushort _minTemperature = 3900;
        private TimeSpan _maximumTemperatureSmoothingDuration = TimeSpan.FromSeconds(3);
        private TimeSpan _temperatureTransitionDuration = TimeSpan.FromMinutes(90);
        private TimeSpan _temperatureUpdateInterval = TimeSpan.FromMinutes(1);
        private TimeSpan _gammaPollingInterval = TimeSpan.FromSeconds(5);
        private TimeSpan _checkForUpdatesInterval = TimeSpan.FromHours(6);
        private TimeSpan _internetSyncInterval = TimeSpan.FromHours(6);
        private TimeSpan _sunriseTime = new TimeSpan(7, 20, 0);
        private TimeSpan _sunsetTime = new TimeSpan(16, 30, 0);
        private GeoInfo _geoInfo;
        private Hotkey _toggleHotkey;
        private Hotkey _togglePollingHotkey;
        private Proxy _proxy;

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

        public bool IsCheckForUpdatedEnabled
        {
            get { return _isCheckForUpdatedEnabled; }
            set { Set(ref _isCheckForUpdatedEnabled, value); }
        }

        public bool IsInternetSyncEnabled
        {
            get { return _isInternetSyncEnabled; }
            set { Set(ref _isInternetSyncEnabled, value); }
        }

        public bool IsGeoInfoLocked
        {
            get { return _isGeoInfoLocked; }
            set { Set(ref _isGeoInfoLocked, value); }
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

        public TimeSpan CheckForUpdatesInterval
        {
            get { return _checkForUpdatesInterval; }
            set { Set(ref _checkForUpdatesInterval, value); }
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
                }

                Set(ref _togglePollingHotkey, value);
            }
        }

        public Proxy Proxy
        {
            get { return _proxy; }
            set { Set(ref _proxy, value); }
        }

        public FileSettingsService()
        {
            // Portable
            if (Environment.GetCommandLineArgs().ContainsInvariant("-portable"))
            {
                Configuration.StorageSpace = StorageSpace.Instance;
                Configuration.SubDirectoryPath = "";
                Configuration.FileName = "Configuration.dat";
            }
            // Not portable
            else
            {
                Configuration.StorageSpace = StorageSpace.SyncedUserDomain;
                Configuration.SubDirectoryPath = "LightBulb";
                Configuration.FileName = "Configuration.dat";
            }

            // Update global configuration when important settings change
            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Proxy))
                    UpdateProxy();
            };
        }

        private void UpdateProxy()
        {
            // Proxy
            if (Proxy == null)
            {
                WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();
                var asWebProxy = WebRequest.DefaultWebProxy as WebProxy;
                if (asWebProxy != null)
                    asWebProxy.UseDefaultCredentials = true;
            }
            else if (Proxy.Host.IsBlank())
            {
                WebRequest.DefaultWebProxy = null;
            }
            else
            {
                var proxy = new WebProxy(Proxy.Host, Proxy.Port);

                if (Proxy.Username.IsBlank())
                {
                    proxy.UseDefaultCredentials = true;
                }
                else
                {
                    proxy.UseDefaultCredentials = false;
                    proxy.Credentials = new NetworkCredential(Proxy.Username, Proxy.Password);
                }

                WebRequest.DefaultWebProxy = proxy;
            }
        }

        public new void Load() => TryLoad();

        public new void Save() => TrySave();
    }
}