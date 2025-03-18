using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.Core;
using LightBulb.Framework;
using LightBulb.Models;
using LightBulb.PlatformInterop;
using LightBulb.Utils;
using Microsoft.Win32;

namespace LightBulb.Services;

[ObservableObject]
public partial class SettingsService() : SettingsBase(GetFilePath(), SerializerContext.Default)
{
    private readonly RegistrySwitch<int> _extendedGammaRangeSwitch = new(
        RegistryHive.LocalMachine,
        @"Software\Microsoft\Windows NT\CurrentVersion\ICM",
        "GdiICMGammaRange",
        256
    );

    private readonly RegistrySwitch<string> _autoStartSwitch = new(
        RegistryHive.CurrentUser,
        @"Software\Microsoft\Windows\CurrentVersion\Run",
        Program.Name,
        $"\"{Program.ExecutableFilePath}\" {StartOptions.IsInitiallyHiddenArgument}"
    );

    [ObservableProperty]
    public partial bool IsFirstTimeExperienceEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool IsUkraineSupportMessageEnabled { get; set; } = true;

    [ObservableProperty]
    [JsonIgnore] // comes from registry
    public partial bool IsExtendedGammaRangeUnlocked { get; set; }

    // General

    public double MinimumTemperature => 500;

    public double MaximumTemperature => 20_000;

    public double MinimumBrightness => 0.1;

    public double MaximumBrightness => 1;

    [ObservableProperty]
    public partial ColorConfiguration DayConfiguration { get; set; } = new(6600, 1);

    [ObservableProperty]
    public partial ColorConfiguration NightConfiguration { get; set; } = new(3900, 0.85);

    [ObservableProperty]
    public partial TimeSpan ConfigurationTransitionDuration { get; set; } =
        TimeSpan.FromMinutes(40);

    [ObservableProperty]
    public partial double ConfigurationTransitionOffset { get; set; }

    [ObservableProperty]
    public partial TimeSpan ConfigurationSmoothingMaxDuration { get; set; } =
        TimeSpan.FromSeconds(5);

    // Location

    [ObservableProperty]
    public partial bool IsManualSunriseSunsetEnabled { get; set; } = true;

    [ObservableProperty]
    [JsonPropertyName("ManualSunriseTime")]
    public partial TimeOnly ManualSunrise { get; set; } = new(07, 20);

    [ObservableProperty]
    [JsonPropertyName("ManualSunsetTime")]
    public partial TimeOnly ManualSunset { get; set; } = new(16, 30);

    [ObservableProperty]
    public partial GeoLocation? Location { get; set; }

    // Advanced

    [ObservableProperty]
    public partial ThemeVariant Theme { get; set; }

    [ObservableProperty]
    [JsonIgnore] // comes from registry
    public partial bool IsAutoStartEnabled { get; set; }

    [ObservableProperty]
    public partial bool IsAutoUpdateEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool IsDefaultToDayConfigurationEnabled { get; set; }

    [ObservableProperty]
    public partial bool IsConfigurationSmoothingEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool IsPauseWhenFullScreenEnabled { get; set; }

    [ObservableProperty]
    public partial bool IsGammaPollingEnabled { get; set; }

    // Application whitelist

    [ObservableProperty]
    public partial bool IsApplicationWhitelistEnabled { get; set; }

    [ObservableProperty]
    public partial IReadOnlyList<ExternalApplication>? WhitelistedApplications { get; set; }

    // HotKeys

    [ObservableProperty]
    public partial HotKey FocusWindowHotKey { get; set; }

    [ObservableProperty]
    public partial HotKey ToggleHotKey { get; set; }

    [ObservableProperty]
    public partial HotKey IncreaseTemperatureOffsetHotKey { get; set; }

    [ObservableProperty]
    public partial HotKey DecreaseTemperatureOffsetHotKey { get; set; }

    [ObservableProperty]
    public partial HotKey IncreaseBrightnessOffsetHotKey { get; set; }

    [ObservableProperty]
    public partial HotKey DecreaseBrightnessOffsetHotKey { get; set; }

    [ObservableProperty]
    public partial HotKey ResetConfigurationOffsetHotKey { get; set; }

    public override void Reset()
    {
        base.Reset();

        // Don't reset the first-time experience
        IsFirstTimeExperienceEnabled = false;
        IsUkraineSupportMessageEnabled = false;

        // Trigger UI updates
        OnPropertyChanged(string.Empty);
    }

    public override void Save()
    {
        // Disallow auto-start in debug mode to make things simpler
#if DEBUG
        IsAutoStartEnabled = false;
#endif

        base.Save();

        // Update values in the registry
        try
        {
            _extendedGammaRangeSwitch.IsSet = IsExtendedGammaRangeUnlocked;
            _autoStartSwitch.IsSet = IsAutoStartEnabled;
        }
        catch (Win32Exception)
        {
            // This can happen if the user doesn't have the necessary permissions to update
            // the corresponding registry keys, and privilege elevation has failed.
            // Throwing an exception here is very messy, so we'll just ignore it.
            // https://github.com/Tyrrrz/LightBulb/issues/335
        }

        // Trigger UI updates
        OnPropertyChanged(string.Empty);
    }

    public override bool Load()
    {
        var wasLoaded = base.Load();

        // Get values from the registry
        IsExtendedGammaRangeUnlocked = _extendedGammaRangeSwitch.IsSet;
        IsAutoStartEnabled = _autoStartSwitch.IsSet;

        // Trigger UI updates
        OnPropertyChanged(string.Empty);

        return wasLoaded;
    }
}

public partial class SettingsService
{
    private static string GetFilePath()
    {
        var isInstalled = File.Exists(Path.Combine(Program.ExecutableDirPath, ".installed"));

        // Prefer storing settings in appdata when installed or when the current directory is write-protected
        if (isInstalled || !DirectoryEx.CheckWriteAccess(Program.ExecutableDirPath))
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Program.Name,
                "Settings.json"
            );
        }

        // Otherwise, store them in the current directory
        return Path.Combine(Program.ExecutableDirPath, "Settings.json");
    }
}

public partial class SettingsService
{
    [JsonSerializable(typeof(SettingsService))]
    private partial class SerializerContext : JsonSerializerContext;
}
