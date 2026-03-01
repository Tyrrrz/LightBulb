using System.Collections.Generic;

namespace LightBulb.Localization;

public partial class LocalizationManager
{
    private static readonly IReadOnlyDictionary<string, string> EnglishLocalization =
        new Dictionary<string, string>
        {
            // Dashboard
            [nameof(SunsetLabel)] = "Sunset",
            [nameof(SunriseLabel)] = "Sunrise",
            [nameof(SunsetTransitionStartsAt)] = "Sunset transition starts at",
            [nameof(SunriseTransitionStartsAt)] = "Sunrise transition starts at",
            [nameof(AndEndsAt)] = "and ends at",
            [nameof(OffsetTooltipHeader)] =
                "Current temperature and brightness values are adjusted by an offset:",
            [nameof(TemperatureOffsetLabel)] = "Temperature offset:",
            [nameof(BrightnessOffsetLabel)] = "Brightness offset:",
            [nameof(ClickToResetLabel)] = "Click to reset",
            [nameof(OffsetLabel)] = "offset",
            // Main window
            [nameof(ToggleLightBulbTooltip)] = "Toggle LightBulb on/off",
            [nameof(HideToTrayTooltip)] = "Hide LightBulb to the system tray",
            [nameof(PreviewText)] = "PREVIEW",
            [nameof(StopPreviewTooltip)] = "Stop preview",
            [nameof(StartPreviewTooltip)] = "Preview 24-hour cycle",
            [nameof(SettingsText)] = "SETTINGS",
            [nameof(OpenSettingsTooltip)] = "Open settings",
            [nameof(AboutText)] = "ABOUT",
            [nameof(OpenGitHubTooltip)] = "Open LightBulb on GitHub",
            // Settings dialog
            [nameof(ResetButton)] = "RESET",
            [nameof(ResetTooltip)] = "Reset all settings to their defaults",
            [nameof(CancelButton)] = "CANCEL",
            [nameof(SaveButton)] = "SAVE",
            // Settings tabs
            [nameof(GeneralTabName)] = "General",
            [nameof(LocationTabName)] = "Location",
            [nameof(AdvancedTabName)] = "Advanced",
            [nameof(AppWhitelistTabName)] = "Application whitelist",
            [nameof(HotkeysTabName)] = "Hotkeys",
            // Advanced settings tab
            [nameof(ThemeLabel)] = "Theme",
            [nameof(ThemeTooltip)] = "Preferred user interface theme",
            [nameof(LanguageLabel)] = "Language",
            [nameof(LanguageTooltip)] = "Preferred user interface language",
            [nameof(StartWithWindowsLabel)] = "Start with Windows",
            [nameof(StartWithWindowsTooltip)] = "Launch LightBulb at Windows startup",
            [nameof(AutoUpdateLabel)] = "Auto-update",
            [nameof(AutoUpdateTooltip)] =
                "Keep LightBulb updated by automatically installing new versions as they become available",
            [nameof(DefaultToDayConfigLabel)] = "Default to day-time configuration",
            [nameof(DefaultToDayConfigTooltip)] =
                "When LightBulb is disabled or paused, restore the configured day-time temperature and brightness instead of the default monitor gamma",
            [nameof(PauseWhenFullscreenLabel)] = "Pause when fullscreen",
            [nameof(PauseWhenFullscreenTooltip)] =
                "Pause LightBulb when any fullscreen window is in the foreground",
            [nameof(GammaSmoothingLabel)] = "Gamma smoothing",
            [nameof(GammaSmoothingTooltip)] =
                "Transition slowly when enabling or disabling LightBulb to give time for eyes to adjust",
            [nameof(GammaPollingLabel)] = "Gamma polling",
            [nameof(GammaPollingTooltip)] =
                "Force-refresh monitor gamma at regular intervals to prevent other programs from overriding it",
            // General settings tab
            [nameof(DayTemperatureLabel)] = "Day-time color temperature:",
            [nameof(DayTemperatureTooltip)] = "Color temperature during the day",
            [nameof(NightTemperatureLabel)] = "Night-time color temperature:",
            [nameof(NightTemperatureTooltip)] = "Color temperature during the night",
            [nameof(DayBrightnessLabel)] = "Day-time brightness:",
            [nameof(DayBrightnessTooltip)] =
                "Brightness during the day\n\nNote that this brightness setting applies to the color gamma, not to the actual brightness of the monitor.\nIf your computer is already capable of auto-adjusting screen brightness based on lighting conditions (common for laptops), then it's recommended to disable LightBulb's brightness control by keeping both brightness settings at 100%.",
            [nameof(NightBrightnessLabel)] = "Night-time brightness:",
            [nameof(NightBrightnessTooltip)] =
                "Brightness during the night\n\nNote that this brightness setting applies to the color gamma, not to the actual brightness of the monitor.\nIf your computer is already capable of auto-adjusting screen brightness based on lighting conditions (common for laptops), then it's recommended to disable LightBulb's brightness control by keeping both brightness settings at 100%.",
            [nameof(TransitionDurationLabel)] = "Transition duration:",
            [nameof(TransitionDurationTooltip)] =
                "Duration of time it takes to switch between day-time and night-time configurations",
            [nameof(TransitionOffsetLabel)] = "Transition offset:",
            [nameof(TransitionOffsetTooltip)] =
                "Offset that specifies how early or late the transition starts, relative to the sunrise and sunset",
            // Location settings tab
            [nameof(SolarConfigLabel)] = "Solar configuration:",
            [nameof(ManualLabel)] = "Manual",
            [nameof(ManualTooltip)] = "Configure sunrise and sunset manually",
            [nameof(LocationBasedLabel)] = "Location-based",
            [nameof(LocationBasedTooltip)] =
                "Configure your location and use it to automatically calculate the sunrise and sunset times",
            [nameof(SunriseTimeLabel)] = "Sunrise:",
            [nameof(SunsetTimeLabel)] = "Sunset:",
            [nameof(YourLocationLabel)] = "Your location:",
            [nameof(AutoDetectLocationTooltip)] =
                "Try to detect the location automatically based on your IP address",
            [nameof(LocationQueryTooltip)] =
                "Specify your location using geographic coordinates or a search query\n\nExamples of valid inputs:\n**41.25, -120.9762**\n**41.25°N, 120.9762°W**\n**New York, USA**\n**Germany**",
            [nameof(SetLocationTooltip)] = "Set location",
            [nameof(LocationErrorText)] = "Error resolving location, try again",
            // Hot key settings tab
            [nameof(ToggleLightBulbHotkeyLabel)] = "Toggle LightBulb",
            [nameof(ToggleLightBulbHotkeyTooltip)] = "Global hotkey to toggle LightBulb on/off",
            [nameof(ToggleWindowLabel)] = "Toggle window",
            [nameof(ToggleWindowHotkeyTooltip)] =
                "Global hotkey to show/hide LightBulb's main window",
            [nameof(IncreaseTemperatureOffsetLabel)] = "Temperature offset ↑",
            [nameof(IncreaseTemperatureOffsetTooltip)] =
                "Global hotkey to increase the current temperature offset",
            [nameof(DecreaseTemperatureOffsetLabel)] = "Temperature offset ↓",
            [nameof(DecreaseTemperatureOffsetTooltip)] =
                "Global hotkey to decrease the current temperature offset",
            [nameof(IncreaseBrightnessOffsetLabel)] = "Brightness offset ↑",
            [nameof(IncreaseBrightnessOffsetTooltip)] =
                "Global hotkey to increase the current brightness offset",
            [nameof(DecreaseBrightnessOffsetLabel)] = "Brightness offset ↓",
            [nameof(DecreaseBrightnessOffsetTooltip)] =
                "Global hotkey to decrease the current brightness offset",
            [nameof(ResetOffsetLabel)] = "Reset offset",
            [nameof(ResetOffsetHotkeyTooltip)] =
                "Global hotkey to reset the current temperature and brightness offsets",
            // Application whitelist settings tab
            [nameof(AppWhitelistLabel)] = "Application whitelist",
            [nameof(RefreshAppsTooltip)] = "Refresh running applications",
            [nameof(PauseForWhitelistedTooltip)] =
                "Pause LightBulb when one of the selected applications is in the foreground",
            // Dialog messages
            [nameof(UpdateAvailableTitle)] = "Update available",
            [nameof(UpdateAvailableMessage)] =
                "Update to {0} v{1} has been downloaded.\nDo you want to install it now?",
            [nameof(InstallButton)] = "INSTALL",
            [nameof(CloseButton)] = "CLOSE",
            [nameof(UkraineSupportTitle)] = "Thank you for supporting Ukraine!",
            [nameof(UkraineSupportMessage)] =
                "As Russia wages a genocidal war against my country, I'm grateful to everyone who continues to stand with Ukraine in our fight for freedom.\n\nClick LEARN MORE to find ways that you can help.",
            [nameof(LearnMoreButton)] = "LEARN MORE",
            [nameof(UnstableBuildTitle)] = "Unstable build warning",
            [nameof(UnstableBuildMessage)] =
                "You're using a development build of {0}. These builds are not thoroughly tested and may contain bugs.\n\nAuto-updates are disabled for development builds. If you want to switch to a stable release, please download it manually.",
            [nameof(SeeReleasesButton)] = "SEE RELEASES",
            [nameof(LimitedGammaRangeTitle)] = "Limited gamma range",
            [nameof(LimitedGammaRangeMessage)] =
                "{0} has detected that extended gamma range controls are not enabled on this system.\nThis may cause some color configurations to not work correctly.\n\nPress FIX to unlock the gamma range. Administrator privileges may be required.",
            [nameof(FixButton)] = "FIX",
            [nameof(WelcomeTitle)] = "Welcome!",
            [nameof(WelcomeMessage)] =
                "Thank you for installing {0}!\nTo get the most personalized experience, please set your preferred solar configuration.\n\nPress OK to open settings.",
            [nameof(OkButton)] = "OK",
        };
}
