using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LightBulb.Services.Interfaces;

namespace LightBulb.ViewModels
{
    public class GeneralSettingsViewModel : ViewModelBase
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
        /// Temperature switch duration in minutes
        /// </summary>
        public double TemperatureSwitchDurationMinutes
        {
            get { return SettingsService.TemperatureTransitionDuration.TotalMinutes; }
            set { SettingsService.TemperatureTransitionDuration = TimeSpan.FromMinutes(value); }
        }

        public RelayCommand StartCyclePreviewCommand { get; }
        public RelayCommand<ushort> RequestPreviewTemperatureCommand { get; }

        public GeneralSettingsViewModel(ITemperatureService temperatureService, ISettingsService settingsService)
        {
            // Services
            SettingsService = settingsService;
            _temperatureService = temperatureService;

            // Commands
            RequestPreviewTemperatureCommand = new RelayCommand<ushort>(RequestPreviewTemperature);
            StartCyclePreviewCommand = new RelayCommand(StartCyclePreview,
                () => !_temperatureService.IsCyclePreviewRunning);

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

        private void StartCyclePreview()
        {
            _temperatureService.StartCyclePreview();
        }
    }
}