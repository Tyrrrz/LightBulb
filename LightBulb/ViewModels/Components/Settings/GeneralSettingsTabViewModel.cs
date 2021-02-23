using System;
using LightBulb.Core;
using LightBulb.Services;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels.Components.Settings
{
    public class GeneralSettingsTabViewModel : SettingsTabViewModelBase
    {
        public double NightTemperature
        {
            get => SettingsService.NightConfiguration.Temperature;
            set
            {
                SettingsService.NightConfiguration = new ColorConfiguration(value, NightBrightness);

                if (NightTemperature > DayTemperature)
                    DayTemperature = NightTemperature;
            }
        }

        public double DayTemperature
        {
            get => SettingsService.DayConfiguration.Temperature;
            set
            {
                SettingsService.DayConfiguration = new ColorConfiguration(value, DayBrightness);

                if (DayTemperature < NightTemperature)
                    NightTemperature = DayTemperature;
            }
        }

        public double NightBrightness
        {
            get => SettingsService.NightConfiguration.Brightness;
            set
            {
                SettingsService.NightConfiguration = new ColorConfiguration(NightTemperature, value);

                if (NightBrightness > DayBrightness)
                    DayBrightness = NightBrightness;
            }
        }

        public double DayBrightness
        {
            get => SettingsService.DayConfiguration.Brightness;
            set
            {
                SettingsService.DayConfiguration = new ColorConfiguration(DayTemperature, value);

                if (DayBrightness < NightBrightness)
                    NightBrightness = DayBrightness;
            }
        }

        public TimeSpan ConfigurationTransitionDuration
        {
            get => SettingsService.ConfigurationTransitionDuration;
            set => SettingsService.ConfigurationTransitionDuration = value.Clamp(TimeSpan.Zero, TimeSpan.FromHours(5));
        }

        public double ConfigurationTransitionOffset
        {
            get => SettingsService.ConfigurationTransitionOffset;
            set => SettingsService.ConfigurationTransitionOffset = value.Clamp(0, 1);
        }

        public GeneralSettingsTabViewModel(SettingsService settingsService)
            : base(settingsService, 0, "General")
        {
        }
    }
}