using System;
using System.Collections.Generic;
using System.Linq;
using LightBulb.Framework;
using LightBulb.Localization;
using LightBulb.PlatformInterop;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components.Settings;

public class AdvancedSettingsTabViewModel(
    SettingsService settingsService,
    LocalizationManager localizationManager,
    GammaService gammaService
) : SettingsTabViewModelBase(settingsService, localizationManager, 2)
{
    public override string DisplayName => LocalizationManager.AdvancedTabName;

    public IReadOnlyList<ThemeVariant> AvailableThemes { get; } = Enum.GetValues<ThemeVariant>();

    public ThemeVariant Theme
    {
        get => SettingsService.Theme;
        set => SettingsService.Theme = value;
    }

    public IReadOnlyList<Language> AvailableLanguages { get; } = Enum.GetValues<Language>();

    public Language Language
    {
        get => SettingsService.Language;
        set => SettingsService.Language = value;
    }

    public bool IsAutoStartEnabled
    {
        get => SettingsService.IsAutoStartEnabled;
        set => SettingsService.IsAutoStartEnabled = value;
    }

    public bool IsAutoUpdateAllowed { get; } = StartOptions.Current.IsAutoUpdateAllowed;

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

    // Controller selection — only shown when more than one controller is available
    public IReadOnlyList<IDisplayGammaController> AvailableControllers =>
        gammaService.AvailableControllers;

    public bool IsControllerSelectionVisible => AvailableControllers.Count > 1;

    public IDisplayGammaController? SelectedController
    {
        get =>
            AvailableControllers.FirstOrDefault(c =>
                c.Id == SettingsService.DisplayGammaControllerId
            ) ?? AvailableControllers.FirstOrDefault();
        set => SettingsService.DisplayGammaControllerId = value?.Id;
    }
}
