using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LightBulb.Services.Interfaces;

namespace LightBulb.ViewModels
{
    public sealed class GeneralSettingsViewModel : ViewModelBase
    {
        private readonly ITemperatureService _temperatureService;

        public ISettingsService SettingsService { get; }

        /// <summary>
        /// Enables or disables the preview mode
        /// </summary>
        public bool IsInPreviewMode
        {
            get { return _temperatureService.IsPreviewModeEnabled; }
            set { _temperatureService.IsPreviewModeEnabled = value; }
        }

        /// <summary>
        /// Enables or disables cycle preview mode
        /// </summary>
        public bool IsInCyclePreviewMode
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

        /// <summary>
        /// Temperature switch duration in minutes
        /// </summary>
        public double TemperatureSwitchDurationMinutes
        {
            get { return SettingsService.TemperatureTransitionDuration.TotalMinutes; }
            set { SettingsService.TemperatureTransitionDuration = TimeSpan.FromMinutes(value); }
        }

        public RelayCommand StartStopCyclePreviewCommand { get; }
        public RelayCommand<ushort> RequestPreviewTemperatureCommand { get; }

        public GeneralSettingsViewModel(ITemperatureService temperatureService, ISettingsService settingsService)
        {
            // Services
            SettingsService = settingsService;
            _temperatureService = temperatureService;

            _temperatureService.CyclePreviewStarted +=
                (sender, args) => RaisePropertyChanged(() => IsInCyclePreviewMode);
            _temperatureService.CyclePreviewEnded +=
                (sender, args) => RaisePropertyChanged(() => IsInCyclePreviewMode);

            // Commands
            RequestPreviewTemperatureCommand = new RelayCommand<ushort>(RequestPreviewTemperature);
            StartStopCyclePreviewCommand = new RelayCommand(StartStopCyclePreview);

            // Settings
            SettingsService.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SettingsService.TemperatureTransitionDuration))
                    RaisePropertyChanged(() => TemperatureSwitchDurationMinutes);
            };
        }

        private void RequestPreviewTemperature(ushort temp)
        {
            _temperatureService.RequestPreviewTemperature(temp);
        }

        private void StartStopCyclePreview()
        {
            IsInCyclePreviewMode = !IsInCyclePreviewMode;
        }
    }
}