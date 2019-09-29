using System;
using LightBulb.Models;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components
{
    public class GeneralSettingsTabViewModel : SettingsTabViewModel
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

        public GeneralSettingsTabViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;

            // Set display name
            DisplayName = "General settings";

            // HACK: when settings change - fire property changed event for all properties in this view model
            _settingsService.PropertyChanged += (sender, args) => NotifyOfPropertyChange(null);
        }
    }
}