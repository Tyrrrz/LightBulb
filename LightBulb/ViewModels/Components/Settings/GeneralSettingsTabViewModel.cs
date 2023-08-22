using System;
using LightBulb.Core;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components.Settings;

public class GeneralSettingsTabViewModel : SettingsTabViewModelBase
{
    public double NightTemperature
    {
        get => SettingsService.NightConfiguration.Temperature;
        set
        {
            SettingsService.NightConfiguration = new ColorConfiguration(
                Math.Clamp(
                    value,
                    SettingsService.MinimumTemperature,
                    SettingsService.MaximumTemperature
                ),
                NightBrightness
            );

            if (NightTemperature > DayTemperature)
                DayTemperature = NightTemperature;
        }
    }

    public double DayTemperature
    {
        get => SettingsService.DayConfiguration.Temperature;
        set
        {
            SettingsService.DayConfiguration = new ColorConfiguration(
                Math.Clamp(
                    value,
                    SettingsService.MinimumTemperature,
                    SettingsService.MaximumTemperature
                ),
                DayBrightness
            );

            if (DayTemperature < NightTemperature)
                NightTemperature = DayTemperature;
        }
    }

    public double NightBrightness
    {
        get => SettingsService.NightConfiguration.Brightness;
        set
        {
            SettingsService.NightConfiguration = new ColorConfiguration(
                NightTemperature,
                Math.Clamp(
                    value,
                    SettingsService.MinimumBrightness,
                    SettingsService.MaximumBrightness
                )
            );

            if (NightBrightness > DayBrightness)
                DayBrightness = NightBrightness;
        }
    }

    public double DayBrightness
    {
        get => SettingsService.DayConfiguration.Brightness;
        set
        {
            SettingsService.DayConfiguration = new ColorConfiguration(
                DayTemperature,
                Math.Clamp(
                    value,
                    SettingsService.MinimumBrightness,
                    SettingsService.MaximumBrightness
                )
            );

            if (DayBrightness < NightBrightness)
                NightBrightness = DayBrightness;
        }
    }

    public TimeSpan ConfigurationTransitionDuration
    {
        get => SettingsService.ConfigurationTransitionDuration;
        set =>
            SettingsService.ConfigurationTransitionDuration = TimeSpan.FromHours(
                Math.Clamp(value.TotalHours, 0, 5)
            );
    }

    public double ConfigurationTransitionOffset
    {
        get => SettingsService.ConfigurationTransitionOffset;
        set => SettingsService.ConfigurationTransitionOffset = Math.Clamp(value, 0, 1);
    }

    public GeneralSettingsTabViewModel(SettingsService settingsService)
        : base(settingsService, 0, "General") { }
}
