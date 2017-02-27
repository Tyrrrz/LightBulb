using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LightBulb.Services.Interfaces;
using LightBulb.ViewModels.Interfaces;

namespace LightBulb.ViewModels
{
    public class GeneralSettingsViewModel : ViewModelBase, IGeneralSettingsViewModel
    {
        private readonly ITemperatureService _temperatureService;

        /// <inheritdoc />
        public ISettingsService SettingsService { get; }

        /// <inheritdoc />
        public bool IsPreviewModeEnabled
        {
            get { return _temperatureService.IsPreviewModeEnabled; }
            set { _temperatureService.IsPreviewModeEnabled = value; }
        }

        /// <inheritdoc />
        public bool IsCyclePreviewModeEnabled
        {
            get { return _temperatureService.IsCyclePreviewRunning; }
            set
            {
                if (value)
                    _temperatureService.StartCyclePreview();
                else
                    _temperatureService.StopCyclePreview();
            }
        }

        public RelayCommand StartStopCyclePreviewCommand { get; }
        public RelayCommand<ushort> RequestPreviewTemperatureCommand { get; }

        public GeneralSettingsViewModel(ITemperatureService temperatureService, ISettingsService settingsService)
        {
            // Services
            SettingsService = settingsService;
            _temperatureService = temperatureService;

            _temperatureService.CyclePreviewStarted +=
                (sender, args) => RaisePropertyChanged(() => IsCyclePreviewModeEnabled);
            _temperatureService.CyclePreviewEnded +=
                (sender, args) => RaisePropertyChanged(() => IsCyclePreviewModeEnabled);

            // Commands
            RequestPreviewTemperatureCommand = new RelayCommand<ushort>(RequestPreviewTemperature);
            StartStopCyclePreviewCommand = new RelayCommand(StartStopCyclePreview);
        }

        private void RequestPreviewTemperature(ushort temp)
        {
            _temperatureService.RequestPreviewTemperature(temp);
        }

        private void StartStopCyclePreview()
        {
            IsCyclePreviewModeEnabled = !IsCyclePreviewModeEnabled;
        }
    }
}