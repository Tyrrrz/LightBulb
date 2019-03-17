using System;
using LightBulb.Models;
using LightBulb.Services;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public class GeneralSettingsViewModel : PropertyChangedBase
    {
        private readonly SettingsService _settingsService;

        public ColorTemperature MinColorTemperature
        {
            get => _settingsService.MinTemperature;
            set
            {
                _settingsService.MinTemperature = value;

                if (MinColorTemperature > MaxColorTemperature)
                    MaxColorTemperature = MinColorTemperature;
            }
        }

        public ColorTemperature MaxColorTemperature
        {
            get => _settingsService.MaxTemperature;
            set
            {
                _settingsService.MaxTemperature = value;

                if (MaxColorTemperature < MinColorTemperature)
                    MinColorTemperature = MaxColorTemperature;
            }
        }

        public TimeSpan TemperatureTransitionDuration
        {
            get => _settingsService.TemperatureTransitionDuration;
            set => _settingsService.TemperatureTransitionDuration = value;
        }

        public GeneralSettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;

            // HACK: when settings service changes - fire property changed event for all properties
            _settingsService.PropertyChanged += (sender, args) => NotifyOfPropertyChange(null);
        }
    }
}