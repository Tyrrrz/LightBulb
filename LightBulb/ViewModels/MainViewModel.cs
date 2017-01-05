using System.Timers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LightBulb.Services;

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

        private void UpdateTemperature()
        {
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