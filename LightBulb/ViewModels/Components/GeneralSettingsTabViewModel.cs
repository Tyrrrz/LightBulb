using System;
using LightBulb.Models;
using LightBulb.Services;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels.Components
{
    public class GeneralSettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly SettingsService _settingsService;

        public double NightTemperature
        {
            get => _settingsService.NightConfiguration.Temperature;
            set
            {
                _settingsService.NightConfiguration = new ColorConfiguration(value.Clamp(1000.0, 10000.0), NightBrightness);

                if (NightTemperature > DayTemperature)
                    DayTemperature = NightTemperature;
            }
        }

        public double DayTemperature
        {
            get => _settingsService.DayConfiguration.Temperature;
            set
            {
                _settingsService.DayConfiguration = new ColorConfiguration(value.Clamp(1000.0, 10000.0), DayBrightness);

                if (DayTemperature < NightTemperature)
                    NightTemperature = DayTemperature;
            }
        }

        public double NightBrightness
        {
            get => _settingsService.NightConfiguration.Brightness;
            set
            {
                _settingsService.NightConfiguration = new ColorConfiguration(NightTemperature, value.Clamp(0.1, 1.0));

                if (NightBrightness > DayBrightness)
                    DayBrightness = NightBrightness;
            }
        }

        public double DayBrightness
        {
            get => _settingsService.DayConfiguration.Brightness;
            set
            {
                _settingsService.DayConfiguration = new ColorConfiguration(DayTemperature, value.Clamp(0.1, 1.0));

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
        }
    }
}