using System;
using LightBulb.Core;
using LightBulb.Localization;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components.Settings;

public class GeneralSettingsTabViewModel(
    SettingsService settingsService,
    LocalizationManager localizationManager
) : SettingsTabViewModelBase(settingsService, localizationManager, 0, "General")
{
    public override string DisplayName => LocalizationManager.GeneralTabName;

    // This value is used for slider bounds, but it's not an actual restriction
    public double RecommendedMaximumDayTemperature => Math.Max(6600, DayTemperature);

    // This value is used for slider bounds, but it's not an actual restriction
    public double RecommendedMinimumDayTemperature => Math.Min(2500, DayTemperature);

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

    // This value is used for slider bounds, but it's not an actual restriction
    public double RecommendedMaximumNightTemperature => Math.Max(6600, NightTemperature);

    // This value is used for slider bounds, but it's not an actual restriction
    public double RecommendedMinimumNightTemperature => Math.Min(2500, NightTemperature);

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
}
