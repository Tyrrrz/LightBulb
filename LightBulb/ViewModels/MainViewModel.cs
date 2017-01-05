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

        private readonly DispatcherTimer _updateTimer;
        private readonly DispatcherTimer _pollingTimer;
        private readonly DispatcherTimer _disableTimer;

        public Settings Settings => Settings.Default;

        private bool _isEnabled = true;
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

        private bool _isPreviewModeEnabled;
        public bool IsPreviewModeEnabled
        {
            get { return _isPreviewModeEnabled; }
            set
            {
                Set(ref _isPreviewModeEnabled, value);
                UpdateGamma();
            }
        }

        private ushort _previewTemperature;
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

        private ushort _currentTemperature = 6500;
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

        public MainViewModel(WinApiService winApiService)
        {
            _winApiService = winApiService;

            _updateTimer = new DispatcherTimer();
            _updateTimer.Tick += (sender, args) => CurrentTemperature = GetTemperature(DateTime.Now);

            _pollingTimer = new DispatcherTimer();
            _pollingTimer.Tick += (sender, args) => UpdateGamma();

            _disableTimer = new DispatcherTimer();
            _disableTimer.Tick += (sender, args) => IsEnabled = true;

            Settings.PropertyChanged += (sender, args) => LoadSettings();

            DisableTemporarilyCommand = new RelayCommand<double>(DisableTemporarily);
            RestoreOriginalCommand = new RelayCommand(() => _winApiService.RestoreOriginal());
            RestoreDefaultCommand = new RelayCommand(() => _winApiService.RestoreDefault());

            // Init
            _winApiService.RestoreDefault();
            LoadSettings();
            _updateTimer.Start();
        }

        private void LoadSettings()
        {
            _pollingTimer.IsEnabled = Settings.IsPollingEnabled;
            _updateTimer.Interval = Settings.UpdateInterval;
            _pollingTimer.Interval = Settings.PollingInterval;
            CurrentTemperature = GetTemperature(DateTime.Now);
        }

        public ushort GetTemperature(DateTime dt)
        {
            ushort minTemp = Settings.MinTemperature;
            ushort maxTemp = Settings.MaxTemperature;
            int diff = maxTemp - minTemp;
            double timeNorm = (dt.Hour + dt.Minute/60d + dt.Second/3600d)/24d;
            double tempNorm = Math.Sin(-Math.PI/2 + 2*timeNorm*Math.PI);
            double temp = minTemp + diff/2 + diff*tempNorm/2;
            return (ushort) temp.RoundToInt().Clamp(ushort.MinValue, ushort.MaxValue);
        }

        public void UpdateGamma()
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

        public void DisableTemporarily(double ms)
        {
            _disableTimer.Stop();
            IsEnabled = false;
            _disableTimer.Interval = TimeSpan.FromMilliseconds(ms);
            _disableTimer.Start();
        }
    }
}