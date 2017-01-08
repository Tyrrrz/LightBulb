using System;
using System.Timers;
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
        private readonly GammaControlService _gammaControlService;
        private readonly TemperatureService _temperatureService;

        private readonly Timer _temperatureUpdateTimer;
        private readonly Timer _pollingTimer;
        private readonly Timer _cyclePreviewTimer;
        private readonly Timer _disableTemporarilyTimer;

        private bool _isEnabled;
        private bool _isPreviewModeEnabled;
        private string _statusText;
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
                if (value) _disableTemporarilyTimer.Enabled = false;
                if (value && !IsPreviewModeEnabled) UpdateTemperature();

                _temperatureUpdateTimer.Enabled = value;
                _pollingTimer.Enabled = value && Settings.Default.IsPollingEnabled;

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
                if (!value) _cyclePreviewTimer.Enabled = false;
                
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
        public RelayCommand ToggleEnabledCommand { get; }
        public RelayCommand<double> DisableTemporarilyCommand { get; }
        public RelayCommand RestoreOriginalCommand { get; }
        public RelayCommand RestoreDefaultCommand { get; }
        public RelayCommand StartCyclePreviewCommand { get; }

        public MainViewModel(GammaControlService gammaControlService, TemperatureService temperatureService)
        {
            _gammaControlService = gammaControlService;
            _temperatureService = temperatureService;

            // Timers
            _temperatureUpdateTimer = new Timer();
            _temperatureUpdateTimer.Elapsed += (sender, args) => UpdateTemperature();
            _pollingTimer = new Timer();
            _pollingTimer.Elapsed += (sender, args) => UpdateGamma();
            _cyclePreviewTimer = new Timer();
            _cyclePreviewTimer.Interval = 10;
            _cyclePreviewTimer.Elapsed += (sender, args) => CyclePreviewUpdateTemperature();
            _disableTemporarilyTimer = new Timer();
            _disableTemporarilyTimer.Elapsed += (sender, args) => IsEnabled = true;

            // Settings
            Settings.Default.PropertyChanged += (sender, args) => ReloadSettings();
            ReloadSettings();

            // Commands
            ShowMainWindowCommand = new RelayCommand(() =>
            {
                Application.Current.MainWindow.Show();
                Application.Current.MainWindow.Activate();
            });
            ExitApplicationCommand = new RelayCommand(() => Application.Current.ShutdownSafe());
            ToggleEnabledCommand = new RelayCommand(() => IsEnabled = !IsEnabled);
            DisableTemporarilyCommand = new RelayCommand<double>(DisableTemporarily);
            RestoreOriginalCommand = new RelayCommand(() => _gammaControlService.RestoreOriginal());
            RestoreDefaultCommand = new RelayCommand(() => _gammaControlService.RestoreDefault());
            StartCyclePreviewCommand = new RelayCommand(StartCyclePreview, () => !_cyclePreviewTimer.Enabled);

            // Init
            IsEnabled = true;
        }

        private void ReloadSettings()
        {
            _temperatureUpdateTimer.Interval = Settings.Default.TemperatureUpdateInterval.TotalMilliseconds;
            _pollingTimer.Interval = Settings.Default.PollingInterval.TotalMilliseconds;

            if (!Settings.Default.IsPollingEnabled)
                _pollingTimer.Enabled = false;
        }

        private void UpdateStatus()
        {
            if (!IsEnabled)
                StatusText = "Turned off";
            else if (!IsPreviewModeEnabled)
                StatusText = $"Temp: {Temperature}K   Time: {Time:t}";
            else if (_cyclePreviewTimer.Enabled)
                StatusText = $"Temp: {PreviewTemperature}K   Time: {PreviewTime:t}";
            else
                StatusText = $"Temp: {PreviewTemperature}K";
        }

        private void UpdateGamma()
        {
            if (IsEnabled)
            {
                ushort temp = IsPreviewModeEnabled ? PreviewTemperature : Temperature;
                var intensity = ColorIntensity.FromTemperature(temp);
                _gammaControlService.SetDisplayGammaLinear(intensity);
            }
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
            if (!newTemp.IsEither(Settings.Default.MinTemperature, Settings.Default.MaxTemperature) &&
                diff < Settings.Default.TemperatureEpsilon) return;

            Temperature = newTemp;
        }

        private void CyclePreviewUpdateTemperature()
        {
            PreviewTime = PreviewTime.AddHours(0.05);
            ushort currentTemp = PreviewTemperature;
            ushort newTemp = _temperatureService.GetTemperature(PreviewTime);
            int diff = Math.Abs(currentTemp - newTemp);

            // Don't update if difference is too small, unless it's either max or min temperature
            if (!newTemp.IsEither(Settings.Default.MinTemperature, Settings.Default.MaxTemperature) &&
                diff < Settings.Default.TemperatureEpsilon) return;

            PreviewTemperature = newTemp;
            IsPreviewModeEnabled = true;

            // Ending condition
            if ((PreviewTime - Time).TotalHours >= 24)
            {
                _cyclePreviewTimer.Enabled = false;
                IsPreviewModeEnabled = false;
                DispatcherHelper.CheckBeginInvokeOnUI(() => StartCyclePreviewCommand.RaiseCanExecuteChanged());
            }
        }

        private void StartCyclePreview()
        {
            PreviewTime = Time;
            _cyclePreviewTimer.Enabled = true;
        }

        private void DisableTemporarily(double ms)
        {
            _disableTemporarilyTimer.Enabled = false;
            _disableTemporarilyTimer.Interval = ms;
            _disableTemporarilyTimer.Enabled = true;
            IsEnabled = false;
        }

        public void Dispose()
        {
            _temperatureUpdateTimer.Dispose();
            _cyclePreviewTimer.Dispose();
            _disableTemporarilyTimer.Dispose();
        }
    }
}