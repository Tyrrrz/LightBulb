using System;
using LightBulb.Internal;
using LightBulb.Services;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels.Components
{
    public class GeneralSettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly SettingsService _settingsService;

        public double NightColorTemperature
        {
            get => _settingsService.NightTemperature;
            set
            {
                _settingsService.NightTemperature = value.Clamp(1000.0, 10000.0);

                if (NightColorTemperature > DayColorTemperature)
                    DayColorTemperature = NightColorTemperature;
            }
        }

        public double DayColorTemperature
        {
            get => _settingsService.DayTemperature;
            set
            {
                _settingsService.DayTemperature = value.Clamp(1000.0, 10000.0);

                if (DayColorTemperature < NightColorTemperature)
                    NightColorTemperature = DayColorTemperature;
            }
        }

        public double NightBrightness
        {
            get => _settingsService.NightBrightness;
            set
            {
                _settingsService.NightBrightness = value.Clamp(0.1, 1.0);

                if (NightBrightness > DayBrightness)
                    DayBrightness = NightBrightness;
            }
        }

        public double DayBrightness
        {
            get => _settingsService.DayBrightness;
            set
            {
                _settingsService.DayBrightness = value.Clamp(0.1, 1.0);

                if (DayBrightness < NightBrightness)
                    NightBrightness = DayBrightness;
            }
        }

        public TimeSpan TemperatureTransitionDuration
        {
            get => _settingsService.TemperatureTransitionDuration;
            set => _settingsService.TemperatureTransitionDuration = value.Clamp(TimeSpan.Zero, TimeSpan.FromHours(5));
        }

        public GeneralSettingsTabViewModel(SettingsService settingsService)
            : base(0, "General")
        {
            _settingsService = settingsService;

            // HACK: when settings change - fire property changed event for all properties in this view model
            _settingsService.Bind((sender, args) => Refresh());
        }
    }
}