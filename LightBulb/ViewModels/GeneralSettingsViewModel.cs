using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using LightBulb.Services;

namespace LightBulb.ViewModels
{
    public class GeneralSettingsViewModel : ViewModelBase
    {
        private readonly TemperatureService _temperatureService;

        /// <summary>
        /// Enables or disables the preview mode
        /// </summary>
        public bool IsInPreviewMode
        {
            get { return _temperatureService.IsPreviewModeEnabled; }
            set { _temperatureService.IsPreviewModeEnabled = value; }
        }

        /// <summary>
        /// Temperature switch duration in minutes
        /// </summary>
        public double TemperatureSwitchDurationMinutes
        {
            get { return Settings.TemperatureSwitchDuration.TotalMinutes; }
            set { Settings.TemperatureSwitchDuration = TimeSpan.FromMinutes(value); }
        }

        public Settings Settings => Settings.Default;

        public RelayCommand StartCyclePreviewCommand { get; }
        public RelayCommand<ushort> RequestPreviewTemperatureCommand { get; }

        public GeneralSettingsViewModel(TemperatureService temperatureService)
        {
            // Services
            _temperatureService = temperatureService;

            _temperatureService.CyclePreviewEnded += (sender, args) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => StartCyclePreviewCommand.RaiseCanExecuteChanged());
            };

            // Commands
            RequestPreviewTemperatureCommand = new RelayCommand<ushort>(RequestPreviewTemperature);
            StartCyclePreviewCommand = new RelayCommand(StartCyclePreview,
                () => !_temperatureService.IsCyclePreviewRunning);

            // Settings
            Settings.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Settings.TemperatureSwitchDuration))
                    RaisePropertyChanged(() => TemperatureSwitchDurationMinutes);
            };
        }

        private void RequestPreviewTemperature(ushort temp)
        {
            _temperatureService.RequestPreviewTemperature(temp);
        }

        private void StartCyclePreview()
        {
            _temperatureService.CyclePreviewStart();
        }
    }
}