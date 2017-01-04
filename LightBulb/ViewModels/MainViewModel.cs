using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LightBulb.Services;

namespace LightBulb.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly WinApiService _winApiService;

        private ushort _currentTemperature;

        public ushort CurrentTemperature
        {
            get { return _currentTemperature; }
            set
            {
                Set(ref _currentTemperature, value);
                UpdateTemperature();
            }
        }

        public RelayCommand RestoreOriginalCommand { get; }
        public RelayCommand RestoreDefaultCommand { get; }

        public MainViewModel(WinApiService winApiService)
        {
            _winApiService = winApiService;

            RestoreOriginalCommand = new RelayCommand(() => _winApiService.RestoreOriginal());
            RestoreDefaultCommand = new RelayCommand(() => _winApiService.RestoreDefault());
        }

        private void UpdateTemperature()
        {
            _winApiService.SetTemperature(CurrentTemperature);
        }
    }
}