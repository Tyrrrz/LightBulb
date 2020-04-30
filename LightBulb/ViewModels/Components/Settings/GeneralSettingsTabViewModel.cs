﻿using System;
using LightBulb.Domain;
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
                SettingsService.NightConfiguration = new ColorConfiguration(value.Clamp(1000.0, 10000.0), NightBrightness);

                if (NightTemperature > DayTemperature)
                    DayTemperature = NightTemperature;
            }
        }

        public double DayTemperature
        {
            get => SettingsService.DayConfiguration.Temperature;
            set
            {
                SettingsService.DayConfiguration = new ColorConfiguration(value.Clamp(1000.0, 10000.0), DayBrightness);

                if (DayTemperature < NightTemperature)
                    NightTemperature = DayTemperature;
            }
        }

        public double NightBrightness
        {
            get => SettingsService.NightConfiguration.Brightness;
            set
            {
                SettingsService.NightConfiguration = new ColorConfiguration(NightTemperature, value.Clamp(0.1, 1.0));

                if (NightBrightness > DayBrightness)
                    DayBrightness = NightBrightness;
            }
        }

        public double DayBrightness
        {
            get => SettingsService.DayConfiguration.Brightness;
            set
            {
                SettingsService.DayConfiguration = new ColorConfiguration(DayTemperature, value.Clamp(0.1, 1.0));

                if (DayBrightness < NightBrightness)
                    NightBrightness = DayBrightness;
            }
        }

        public TimeSpan ConfigurationTransitionDuration
        {
            get => SettingsService.ConfigurationTransitionDuration;
            set => SettingsService.ConfigurationTransitionDuration = value.Clamp(TimeSpan.Zero, TimeSpan.FromHours(5));
        }

        public GeneralSettingsTabViewModel(SettingsService settingsService)
            : base(settingsService, 0, "General")
        {
        }
    }
}