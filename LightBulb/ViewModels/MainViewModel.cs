using System;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LightBulb.Models;
using LightBulb.Services;
using NegativeLayer.Extensions;

namespace LightBulb.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly WinApiService _winApiService;

        private readonly DispatcherTimer _updateTimer; // handles temperature changes
        private readonly DispatcherTimer _previewUpdateTimer; // handles temperature changes in preview
        private readonly DispatcherTimer _pollingTimer; // updates gamma periodically
        private readonly DispatcherTimer _disableTimer; // "disable for xx" timer

        public Settings Settings => Settings.Default;

        private double _previewHours; // fake hours for 24hr cycle preview

        private bool _isEnabled = true;
        private bool _isPreviewModeEnabled;
        private ushort _previewTemperature;
        private ushort _currentTemperature;

        /// <summary>
        /// Enables or disables the program
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                // Stop the disable timer if set to true
                if (value)
                    _disableTimer.Stop();

                // Enable/disable update timer
                _updateTimer.IsEnabled = value;

                Set(ref _isEnabled, value);
                UpdateGamma();
            }
        }

        /// <summary>
        /// When set to true, the preview temperature controls the gamma
        /// </summary>
        public bool IsPreviewModeEnabled
        {
            get { return _isPreviewModeEnabled; }
            set
            {
                Set(ref _isPreviewModeEnabled, value);
                UpdateGamma();
            }
        }

        /// <summary>
        /// Fake temperature used for preview
        /// </summary>
        public ushort PreviewTemperature
        {
            get { return _previewTemperature; }
            set
            {
                // Discard if change is not sufficient
                if (Math.Abs(value - PreviewTemperature) < Settings.TemperatureEpsilon)
                    return;

                Set(ref _previewTemperature, value);
                UpdateGamma();
            }
        }

        /// <summary>
        /// Actual temperature used to control gamma
        /// </summary>
        public ushort CurrentTemperature
        {
            get { return _currentTemperature; }
            set
            {
                // Discard if change is not sufficient
                if (Math.Abs(value - CurrentTemperature) < Settings.TemperatureEpsilon)
                    return;

                Set(ref _currentTemperature, value);
                UpdateGamma();
            }
        }

        public RelayCommand<double> DisableTemporarilyCommand { get; }
        public RelayCommand RestoreOriginalCommand { get; }
        public RelayCommand RestoreDefaultCommand { get; }
        public RelayCommand PreviewCycleCommand { get; }

        public MainViewModel(WinApiService winApiService)
        {
            _winApiService = winApiService;

            // Update timer
            _updateTimer = new DispatcherTimer();
            _updateTimer.Tick += (sender, args) => CurrentTemperature = GetTemperature(DateTime.Now);

            // Preview update timer
            _previewUpdateTimer = new DispatcherTimer();
            _previewUpdateTimer.Interval = TimeSpan.FromSeconds(1/30d);
            _previewUpdateTimer.Tick += (sender, args) =>
            {
                IsPreviewModeEnabled = true;
                PreviewTemperature = GetTemperature(DateTime.Today.AddHours(_previewHours));
                _previewHours += 0.1;
                if (_previewHours >= 24)
                {
                    _previewHours = 0;
                    _previewUpdateTimer.Stop();
                    IsPreviewModeEnabled = false;
                    PreviewCycleCommand.RaiseCanExecuteChanged();
                }
            };

            // Polling timer
            _pollingTimer = new DispatcherTimer();
            _pollingTimer.Tick += (sender, args) => UpdateGamma();

            // Disable timer
            _disableTimer = new DispatcherTimer();
            _disableTimer.Tick += (sender, args) => IsEnabled = true;

            // Update settings when they are changed
            Settings.PropertyChanged += (sender, args) => LoadSettings();

            // Commands
            DisableTemporarilyCommand = new RelayCommand<double>(DisableTemporarily);
            RestoreOriginalCommand = new RelayCommand(() => _winApiService.RestoreOriginal());
            RestoreDefaultCommand = new RelayCommand(() => _winApiService.RestoreDefault());
            PreviewCycleCommand = new RelayCommand(PreviewCycle, () => !_previewUpdateTimer.IsEnabled);

            // Init
            LoadSettings();
            _updateTimer.Start();
        }

        /// <summary>
        /// Update stuff that depends on settings
        /// </summary>
        private void LoadSettings()
        {
            _pollingTimer.IsEnabled = Settings.IsPollingEnabled;
            _updateTimer.Interval = Settings.UpdateInterval;
            _pollingTimer.Interval = Settings.PollingInterval;
            CurrentTemperature = GetTemperature(DateTime.Now);
        }

        /// <summary>
        /// Get temperature that corresponds to the given time
        /// </summary>
        private ushort GetTemperature(DateTime dt)
        {
            ushort minTemp = Settings.MinTemperature;
            ushort maxTemp = Settings.MaxTemperature;
            int diff = maxTemp - minTemp;
            double timeNorm = dt.TimeOfDay.TotalHours/24d;
            double tempNorm = Math.Sin(-Math.PI/2 + 2*timeNorm*Math.PI);
            double temp = minTemp + diff/2 + diff*tempNorm/2;
            return (ushort) temp.RoundToInt().Clamp(ushort.MinValue, ushort.MaxValue);
        }

        /// <summary>
        /// Update the display gamma, based on temperature
        /// </summary>
        private void UpdateGamma()
        {
            if (!IsEnabled)
            {
                _winApiService.RestoreDefault();
            }
            else
            {
                ushort temp = IsPreviewModeEnabled ? PreviewTemperature : CurrentTemperature;
                var intensity = ColorIntensity.FromTemperature(temp);
                _winApiService.SetDisplayGammaLinear(intensity);
            }
        }

        /// <summary>
        /// Disable the program for the given amount of milliseconds
        /// </summary>
        private void DisableTemporarily(double ms)
        {
            _disableTimer.Stop();
            IsEnabled = false;
            _disableTimer.Interval = TimeSpan.FromMilliseconds(ms);
            _disableTimer.Start();
        }

        /// <summary>
        /// Preview the 24hr cycle in fast-motion
        /// </summary>
        private void PreviewCycle()
        {
            _previewUpdateTimer.Stop();
            _previewHours = 0;
            _previewUpdateTimer.Start();
            PreviewCycleCommand.RaiseCanExecuteChanged();
        }
    }
}