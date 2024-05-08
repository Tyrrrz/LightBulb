using System;
using LightBulb.Models;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components.Settings;

public class AdvancedSettingsTabViewModel(SettingsService settingsService)
    : SettingsTabViewModelBase(settingsService, 2, "Advanced")
{
    public bool IsAutoStartEnabled
    {
        get => SettingsService.IsAutoStartEnabled;
        set => SettingsService.IsAutoStartEnabled = value;
    }

    public Array ThemeArray { get; } = Enum.GetValues<ThemeMode>();

    public ThemeMode Theme
    {
        get => SettingsService.Theme;
        set => SettingsService.Theme = value;
    }

    public bool IsAutoUpdateEnabled
    {
        get => SettingsService.IsAutoUpdateEnabled;
        set => SettingsService.IsAutoUpdateEnabled = value;
    }

    public bool IsDefaultToDayConfigurationEnabled
    {
        get => SettingsService.IsDefaultToDayConfigurationEnabled;
        set => SettingsService.IsDefaultToDayConfigurationEnabled = value;
    }

    public bool IsConfigurationSmoothingEnabled
    {
        get => SettingsService.IsConfigurationSmoothingEnabled;
        set => SettingsService.IsConfigurationSmoothingEnabled = value;
    }

    public bool IsPauseWhenFullScreenEnabled
    {
        get => SettingsService.IsPauseWhenFullScreenEnabled;
        set => SettingsService.IsPauseWhenFullScreenEnabled = value;
    }

    public bool IsGammaPollingEnabled
    {
        get => SettingsService.IsGammaPollingEnabled;
        set => SettingsService.IsGammaPollingEnabled = value;
    }
}
