using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LightBulb.Services.Interfaces;
using LightBulb.ViewModels.Interfaces;

namespace LightBulb.ViewModels
{
    public class GeneralSettingsViewModel : ViewModelBase, IGeneralSettingsViewModel, IDisposable
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
        public bool IsCyclePreviewRunning
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

            _temperatureService.CyclePreviewStarted += TemperatureServiceCyclePreviewStarted;
            _temperatureService.CyclePreviewEnded += TemperatureServiceCyclePreviewEnded;

            // Commands
            RequestPreviewTemperatureCommand = new RelayCommand<ushort>(RequestPreviewTemperature);
            StartStopCyclePreviewCommand = new RelayCommand(StartStopCyclePreview);
        }

        ~GeneralSettingsViewModel()
        {
            Dispose(false);
        }

        private void TemperatureServiceCyclePreviewStarted(object sender, EventArgs args)
        {
            RaisePropertyChanged(() => IsCyclePreviewRunning);
        }

        private void TemperatureServiceCyclePreviewEnded(object sender, EventArgs args)
        {
            RaisePropertyChanged(() => IsCyclePreviewRunning);
        }

        private void RequestPreviewTemperature(ushort temp)
        {
            _temperatureService.RequestPreviewTemperature(temp);
        }

        private void StartStopCyclePreview()
        {
            IsCyclePreviewRunning = !IsCyclePreviewRunning;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _temperatureService.CyclePreviewStarted -= TemperatureServiceCyclePreviewStarted;
                _temperatureService.CyclePreviewEnded -= TemperatureServiceCyclePreviewEnded;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}