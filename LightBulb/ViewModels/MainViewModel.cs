using System;
using System.Timers;
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
        private readonly Timer _disableTimer;

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

                Set(ref _isEnabled, value);
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
                if (Math.Abs(value - PreviewTemperature) < Settings.SmallestTemperatureInterval)
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
                if (Math.Abs(value - CurrentTemperature) < Settings.SmallestTemperatureInterval)
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

            _disableTimer = new Timer();
            _disableTimer.Elapsed += (sender, args) => IsEnabled = true;

            DisableTemporarilyCommand = new RelayCommand<double>(DisableTemporarily);
            RestoreOriginalCommand = new RelayCommand(() => _winApiService.RestoreOriginal());
            RestoreDefaultCommand = new RelayCommand(() => _winApiService.RestoreDefault());
        }

        private ColorIntensity TemperatureToIntensity(ushort temp)
        {
            // http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/

            double tempf = temp/100d;

            double redi;
            double greeni;
            double bluei;

            // Red
            if (tempf <= 66)
                redi = 1;
            else
                redi = (Math.Pow(tempf - 60, -0.1332047592)*329.698727446).Clamp(0, 255)/255d;

            // Green
            if (tempf <= 66)
                greeni = (Math.Log(tempf)*99.4708025861 - 161.1195681661).Clamp(0, 255)/255d;
            else
                greeni = (Math.Pow(tempf - 60, -0.0755148492)*288.1221695283).Clamp(0, 255)/255d;

            // Blue
            if (tempf >= 66)
                bluei = 1;
            else
            {
                if (tempf <= 19)
                    bluei = 0;
                else
                    bluei = (Math.Log(tempf - 10)*138.5177312231 - 305.0447927307).Clamp(0, 255)/255d;
            }

            return new ColorIntensity(redi, greeni, bluei);
        }

        private void UpdateGamma()
        {
            var intensity = TemperatureToIntensity(IsPreviewModeEnabled ? PreviewTemperature : CurrentTemperature);
            _winApiService.SetDisplayGammaLinear(intensity);
        }

        private void DisableTemporarily(double ms)
        {
            _disableTimer.Stop();
            IsEnabled = false;
            _disableTimer.Interval = ms;
            _disableTimer.Start();
        }
    }
}