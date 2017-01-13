using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using LightBulb.Models;
using LightBulb.Services;
using NegativeLayer.Extensions;
using NegativeLayer.WPFExtensions;

namespace LightBulb.ViewModels
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private readonly TemperatureService _temperatureService;
        private readonly GammaControlService _gammaControlService;
        private readonly WindowService _windowService;
        private readonly GeolocationService _geolocationService;

        private readonly SyncedTimer _temperatureUpdateTimer;
        private readonly Timer _pollingTimer;
        private readonly Timer _disableTemporarilyTimer;
        private readonly Timer _cyclePreviewTimer;
        private readonly SyncedTimer _internetSyncTimer;

        private bool _isEnabled;
        private bool _isPreviewModeEnabled;
        private string _statusText;
        private CycleState _cycleState;
        private DateTime _time;
        private DateTime _previewTime;
        private ushort _temperature;
        private ushort _previewTemperature;

        public Settings Settings => Settings.Default;

        /// <summary>
        /// Enables or disables the program
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (!Set(ref _isEnabled, value)) return;
                if (value) _disableTemporarilyTimer.IsEnabled = false;
                if (value && !IsPreviewModeEnabled) UpdateTemperature();

                _temperatureUpdateTimer.IsEnabled = value;
                _pollingTimer.IsEnabled = value && Settings.IsGammaPollingEnabled;

                UpdateGamma();
                UpdateStatus();
            }
        }

        /// <summary>
        /// Enables or disables the preview mode
        /// </summary>
        public bool IsPreviewModeEnabled
        {
            get { return _isPreviewModeEnabled; }
            set
            {
                if (!Set(ref _isPreviewModeEnabled, value)) return;
                
                UpdateGamma();
                UpdateStatus();
            }
        }

        /// <summary>
        /// Status text
        /// </summary>
        public string StatusText
        {
            get { return _statusText; }
            private set { Set(ref _statusText, value); }
        }

        /// <summary>
        /// State of the current cycle
        /// </summary>
        public CycleState CycleState
        {
            get { return _cycleState; }
            private set { Set(ref _cycleState, value); }
        }

        /// <summary>
        /// Time used for temperature calculations
        /// </summary>
        public DateTime Time
        {
            get { return _time; }
            private set
            {
                if (!Set(ref _time, value)) return;

                UpdateStatus();
            }
        }

        /// <summary>
        /// Time used for preview-mode temperature calculations
        /// </summary>
        public DateTime PreviewTime
        {
            get { return _previewTime; }
            private set
            {
                if (!Set(ref _previewTime, value)) return;

                UpdateStatus();
            }
        }

        /// <summary>
        /// Current light temperature
        /// </summary>
        public ushort Temperature
        {
            get { return _temperature; }
            private set
            {
                if (!Set(ref _temperature, value)) return;

                UpdateGamma();
                UpdateStatus();
            }
        }

        /// <summary>
        /// Preview mode light temperature
        /// </summary>
        public ushort PreviewTemperature
        {
            get { return _previewTemperature; }
            set
            {
                if (!Set(ref _previewTemperature, value)) return;
                if (!IsPreviewModeEnabled) return;

                UpdateGamma();
                UpdateStatus();
            }
        }

        // Commands
        public RelayCommand ShowMainWindowCommand { get; }
        public RelayCommand ExitApplicationCommand { get; }
        public RelayCommand AboutCommand { get; }
        public RelayCommand ToggleEnabledCommand { get; }
        public RelayCommand<double> DisableTemporarilyCommand { get; }
        public RelayCommand RestoreOriginalCommand { get; }
        public RelayCommand RestoreDefaultCommand { get; }
        public RelayCommand StartCyclePreviewCommand { get; }

        public MainViewModel(TemperatureService temperatureService, GammaControlService gammaControlService,
            WindowService windowService, GeolocationService geolocationService)
        {
            // Services
            _temperatureService = temperatureService;
            _gammaControlService = gammaControlService;
            _windowService = windowService;
            _geolocationService = geolocationService;

            _windowService.FullScreenStateChanged += (sender, args) => UpdateGamma();

            // Timers
            _temperatureUpdateTimer = new SyncedTimer();
            _temperatureUpdateTimer.Tick += (sender, args) => UpdateTemperature();
            _pollingTimer = new Timer();
            _pollingTimer.Tick += (sender, args) => UpdateGamma();
            _cyclePreviewTimer = new Timer(TimeSpan.FromMilliseconds(10));
            _cyclePreviewTimer.Tick += (sender, args) => CyclePreviewUpdateTemperature();
            _disableTemporarilyTimer = new Timer();
            _disableTemporarilyTimer.Tick += (sender, args) => IsEnabled = true;
            _internetSyncTimer = new SyncedTimer();
            _internetSyncTimer.Tick += async (sender, args) => await InternetSyncAsync();

            // Settings
            Settings.PropertyChanged += (sender, args) => LoadSettings();
            LoadSettings();

            // Commands
            ShowMainWindowCommand = new RelayCommand(() =>
            {
                Application.Current.MainWindow.Show();
                Application.Current.MainWindow.Activate();
                Application.Current.MainWindow.Focus();
            });
            ExitApplicationCommand = new RelayCommand(() => Application.Current.ShutdownSafe());
            AboutCommand = new RelayCommand(() => Process.Start("https://github.com/Tyrrrz/LightBulb"));
            ToggleEnabledCommand = new RelayCommand(() => IsEnabled = !IsEnabled);
            DisableTemporarilyCommand = new RelayCommand<double>(DisableTemporarily);
            RestoreOriginalCommand = new RelayCommand(() => _gammaControlService.RestoreOriginal());
            RestoreDefaultCommand = new RelayCommand(() => _gammaControlService.RestoreDefault());
            StartCyclePreviewCommand = new RelayCommand(StartCyclePreview, () => !_cyclePreviewTimer.IsEnabled);

            // Init
            IsEnabled = true;
        }

        private void LoadSettings()
        {
            // Services
            _windowService.UseEventHooks = Settings.DisableWhenFullscreen;

            // Timers
            _temperatureUpdateTimer.Interval = Settings.TemperatureUpdateInterval;
            _pollingTimer.Interval = Settings.GammaPollingInterval;
            _internetSyncTimer.Interval = Settings.InternetSyncInterval;

            _internetSyncTimer.IsEnabled = Settings.IsInternetTimeSyncEnabled;
            if (!Settings.IsGammaPollingEnabled)
                _pollingTimer.IsEnabled = false;

            // Refresh stuff
            UpdateTemperature();
            if (Settings.IsInternetTimeSyncEnabled)
                InternetSyncAsync().Forget();
        }

        private void UpdateStatus()
        {
            // Not enabled
            if (!IsEnabled)
            {
                CycleState = CycleState.Disabled;
                StatusText = "LightBulb is off";
            }
            // Preview mode (not 24hr cycle preview)
            else if (IsPreviewModeEnabled && !_cyclePreviewTimer.IsEnabled)
            {
                CycleState = CycleState.Disabled;
                StatusText = $"Temp: {PreviewTemperature}K   (preview)";
            }
            // Preview mode (24 hr cycle preview)
            else if (IsPreviewModeEnabled && _cyclePreviewTimer.IsEnabled)
            {
                if (PreviewTemperature >= Settings.MaxTemperature) CycleState = CycleState.Day;
                else if (PreviewTemperature <= Settings.MinTemperature) CycleState = CycleState.Night;
                else CycleState = CycleState.Transition;

                StatusText = $"Temp: {PreviewTemperature}K   Time: {PreviewTime:t}   (preview)";
            }
            // Realtime mode
            else
            {
                if (Temperature >= Settings.MaxTemperature) CycleState = CycleState.Day;
                else if (Temperature <= Settings.MinTemperature) CycleState = CycleState.Night;
                else CycleState = CycleState.Transition;

                StatusText = $"Temp: {Temperature}K   Time: {Time:t}";
            }
        }

        private void UpdateGamma()
        {
            // Check if gamma control is blocked by something
            bool isBlocked = Settings.DisableWhenFullscreen && _windowService.IsForegroundFullScreen;

            // If enabled and not blocked or is in preview mode
            if ((IsEnabled && !isBlocked) || IsPreviewModeEnabled)
            {
                ushort temp = IsPreviewModeEnabled ? PreviewTemperature : Temperature;
                var intensity = ColorIntensity.FromTemperature(temp);
                _gammaControlService.SetDisplayGammaLinear(intensity);
            }
            // When disabled - reset gamma to default
            else
            {
                _gammaControlService.RestoreDefault();
            }
        }

        private void UpdateTemperature()
        {
            Time = DateTime.Now;
            ushort currentTemp = Temperature;
            ushort newTemp = _temperatureService.GetTemperature(Time);
            int diff = Math.Abs(currentTemp - newTemp);

            // Don't update if difference is too small, unless it's either max or min temperature
            if (!newTemp.IsEither(Settings.MinTemperature, Settings.MaxTemperature) &&
                diff < Settings.TemperatureEpsilon) return;

            Temperature = newTemp;
        }

        private void CyclePreviewUpdateTemperature()
        {
            PreviewTime = PreviewTime.AddHours(0.05);
            ushort currentTemp = PreviewTemperature;
            ushort newTemp = _temperatureService.GetTemperature(PreviewTime);
            int diff = Math.Abs(currentTemp - newTemp);

            // Don't update if difference is too small, unless it's either max or min temperature
            if (!newTemp.IsEither(Settings.MinTemperature, Settings.MaxTemperature) &&
                diff < Settings.TemperatureEpsilon) return;

            PreviewTemperature = newTemp;
            IsPreviewModeEnabled = true;

            // Ending condition
            if ((PreviewTime - Time).TotalHours >= 24)
            {
                _cyclePreviewTimer.IsEnabled = false;
                IsPreviewModeEnabled = false;
                DispatcherHelper.CheckBeginInvokeOnUI(() => StartCyclePreviewCommand.RaiseCanExecuteChanged());
            }
        }

        private void StartCyclePreview()
        {
            PreviewTime = Time;
            _cyclePreviewTimer.IsEnabled = true;
        }

        private void DisableTemporarily(double ms)
        {
            _disableTemporarilyTimer.IsEnabled = false;
            _disableTemporarilyTimer.Interval = TimeSpan.FromMilliseconds(ms);
            _disableTemporarilyTimer.IsEnabled = true;
            IsEnabled = false;
        }

        private async Task InternetSyncAsync()
        {
            // Get coordinates
            Settings.GeoInfo = await _geolocationService.GetGeolocationInfoAsync();
            if (Settings.GeoInfo == null) return; // fail

            // Get the sunrise/sunset times
            var solarInfo = await _geolocationService.GetSolarInfoAsync(Settings.GeoInfo);
            if (solarInfo == null) return; // fail

            // Update settings
            if (Settings.IsInternetTimeSyncEnabled)
            {
                Settings.SunriseTime = solarInfo.Sunrise.TimeOfDay;
                Settings.SunsetTime = solarInfo.Sunset.TimeOfDay;
            }
        }

        public void Dispose()
        {
            _temperatureUpdateTimer.Dispose();
            _pollingTimer.Dispose();
            _cyclePreviewTimer.Dispose();
            _disableTemporarilyTimer.Dispose();
            _internetSyncTimer.Dispose();
        }
    }
}