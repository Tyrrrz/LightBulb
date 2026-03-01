using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;

namespace LightBulb.Localization;

public partial class LocalizationManager : ObservableObject, IDisposable
{
    private readonly DisposableCollector _eventRoot = new();

    public LocalizationManager(SettingsService settingsService)
    {
        _eventRoot.Add(
            settingsService.WatchProperty(
                o => o.Language,
                () => Language = settingsService.Language,
                true
            )
        );

        _eventRoot.Add(
            this.WatchProperty(
                o => o.Language,
                () =>
                {
                    foreach (var propertyName in EnglishLocalization.Keys)
                        OnPropertyChanged(propertyName);
                }
            )
        );
    }

    [ObservableProperty]
    public partial Language Language { get; set; } = Language.System;

    private string Get([CallerMemberName] string? key = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        var localization = Language switch
        {
            Language.System =>
                CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName.ToLowerInvariant() switch
                {
                    "ukr" => UkrainianLocalization,
                    "deu" => GermanLocalization,
                    "fra" => FrenchLocalization,
                    "spa" => SpanishLocalization,
                    _ => EnglishLocalization,
                },
            Language.Ukrainian => UkrainianLocalization,
            Language.German => GermanLocalization,
            Language.French => FrenchLocalization,
            Language.Spanish => SpanishLocalization,
            _ => EnglishLocalization,
        };

        if (
            localization.TryGetValue(key, out var value)
            || EnglishLocalization.TryGetValue(key, out value)
        )
        {
            return value;
        }

        return $"Missing localization for '{key}'";
    }

    public void Dispose() => _eventRoot.Dispose();
}

public partial class LocalizationManager
{
    // ---- Dashboard ----

    public string SunsetLabel => Get();
    public string SunriseLabel => Get();
    public string SunsetTransitionStartsAt => Get();
    public string SunriseTransitionStartsAt => Get();
    public string AndEndsAt => Get();
    public string OffsetTooltipHeader => Get();
    public string TemperatureOffsetLabel => Get();
    public string BrightnessOffsetLabel => Get();
    public string ClickToResetLabel => Get();
    public string OffsetLabel => Get();

    // ---- Main window ----

    public string ToggleLightBulbTooltip => Get();
    public string HideToTrayTooltip => Get();
    public string PreviewText => Get();
    public string StopPreviewTooltip => Get();
    public string StartPreviewTooltip => Get();
    public string SettingsText => Get();
    public string OpenSettingsTooltip => Get();
    public string AboutText => Get();
    public string OpenGitHubTooltip => Get();

    // ---- Settings dialog ----

    public string ResetButton => Get();
    public string ResetTooltip => Get();
    public string CancelButton => Get();
    public string SaveButton => Get();

    // ---- Settings tabs ----

    public string GeneralTabName => Get();
    public string LocationTabName => Get();
    public string AdvancedTabName => Get();
    public string AppWhitelistTabName => Get();
    public string HotkeysTabName => Get();

    // ---- Advanced settings tab ----

    public string ThemeLabel => Get();
    public string ThemeTooltip => Get();
    public string LanguageLabel => Get();
    public string LanguageTooltip => Get();
    public string StartWithWindowsLabel => Get();
    public string StartWithWindowsTooltip => Get();
    public string AutoUpdateLabel => Get();
    public string AutoUpdateTooltip => Get();
    public string DefaultToDayConfigLabel => Get();
    public string DefaultToDayConfigTooltip => Get();
    public string PauseWhenFullscreenLabel => Get();
    public string PauseWhenFullscreenTooltip => Get();
    public string GammaSmoothingLabel => Get();
    public string GammaSmoothingTooltip => Get();
    public string GammaPollingLabel => Get();
    public string GammaPollingTooltip => Get();

    // ---- General settings tab ----

    public string DayTemperatureLabel => Get();
    public string DayTemperatureTooltip => Get();
    public string NightTemperatureLabel => Get();
    public string NightTemperatureTooltip => Get();
    public string DayBrightnessLabel => Get();
    public string DayBrightnessTooltip => Get();
    public string NightBrightnessLabel => Get();
    public string NightBrightnessTooltip => Get();
    public string TransitionDurationLabel => Get();
    public string TransitionDurationTooltip => Get();
    public string TransitionOffsetLabel => Get();
    public string TransitionOffsetTooltip => Get();

    // ---- Location settings tab ----

    public string SolarConfigLabel => Get();
    public string ManualLabel => Get();
    public string ManualTooltip => Get();
    public string LocationBasedLabel => Get();
    public string LocationBasedTooltip => Get();
    public string SunriseTimeLabel => Get();
    public string SunsetTimeLabel => Get();
    public string YourLocationLabel => Get();
    public string AutoDetectLocationTooltip => Get();
    public string LocationQueryTooltip => Get();
    public string SetLocationTooltip => Get();
    public string LocationErrorText => Get();

    // ---- Hot key settings tab ----

    public string ToggleLightBulbHotkeyLabel => Get();
    public string ToggleLightBulbHotkeyTooltip => Get();
    public string ToggleWindowLabel => Get();
    public string ToggleWindowHotkeyTooltip => Get();
    public string IncreaseTemperatureOffsetLabel => Get();
    public string IncreaseTemperatureOffsetTooltip => Get();
    public string DecreaseTemperatureOffsetLabel => Get();
    public string DecreaseTemperatureOffsetTooltip => Get();
    public string IncreaseBrightnessOffsetLabel => Get();
    public string IncreaseBrightnessOffsetTooltip => Get();
    public string DecreaseBrightnessOffsetLabel => Get();
    public string DecreaseBrightnessOffsetTooltip => Get();
    public string ResetOffsetLabel => Get();
    public string ResetOffsetHotkeyTooltip => Get();

    // ---- Application whitelist settings tab ----

    public string AppWhitelistLabel => Get();
    public string RefreshAppsTooltip => Get();
    public string PauseForWhitelistedTooltip => Get();

    // ---- Dialog messages ----

    public string UpdateAvailableTitle => Get();
    public string UpdateAvailableMessage => Get();
    public string InstallButton => Get();
    public string CloseButton => Get();
    public string UkraineSupportTitle => Get();
    public string UkraineSupportMessage => Get();
    public string LearnMoreButton => Get();
    public string UnstableBuildTitle => Get();
    public string UnstableBuildMessage => Get();
    public string SeeReleasesButton => Get();
    public string LimitedGammaRangeTitle => Get();
    public string LimitedGammaRangeMessage => Get();
    public string FixButton => Get();
    public string WelcomeTitle => Get();
    public string WelcomeMessage => Get();
    public string OkButton => Get();
}
