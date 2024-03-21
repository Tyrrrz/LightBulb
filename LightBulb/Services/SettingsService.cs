using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.Core;
using LightBulb.Models;
using LightBulb.Utils;
using LightBulb.WindowsApi;
using Microsoft.Win32;

namespace LightBulb.Services;

[INotifyPropertyChanged]
public partial class SettingsService() : SettingsBase(GetFilePath())
{
    private readonly RegistrySwitch<int> _extendedGammaRangeSwitch =
        new(
            RegistryHive.LocalMachine,
            @"Software\Microsoft\Windows NT\CurrentVersion\ICM",
            "GdiICMGammaRange",
            256
        );

    private readonly RegistrySwitch<string> _autoStartSwitch =
        new(
            RegistryHive.CurrentUser,
            @"Software\Microsoft\Windows\CurrentVersion\Run",
            "LightBulb",
            $"\"{Program.ExecutableFilePath}\" {Program.StartHiddenArgument}"
        );

    public bool IsFirstTimeExperienceEnabled { get; set; } = true;

    public bool IsUkraineSupportMessageEnabled { get; set; } = true;

    [JsonIgnore] // comes from registry
    public bool IsExtendedGammaRangeUnlocked { get; set; }

    // General

    public double MinimumTemperature => 500;

    public double MaximumTemperature => 20_000;

    public double MinimumBrightness => 0.1;

    public double MaximumBrightness => 1;

    public ColorConfiguration NightConfiguration { get; set; } = new(3900, 0.85);

    public ColorConfiguration DayConfiguration { get; set; } = new(6600, 1);

    public TimeSpan ConfigurationTransitionDuration { get; set; } = TimeSpan.FromMinutes(40);

    public double ConfigurationTransitionOffset { get; set; }

    // Location

    public bool IsManualSunriseSunsetEnabled { get; set; } = true;

    [JsonPropertyName("ManualSunriseTime")]
    public TimeOnly ManualSunrise { get; set; } = new(07, 20);

    [JsonPropertyName("ManualSunsetTime")]
    public TimeOnly ManualSunset { get; set; } = new(16, 30);

    public GeoLocation? Location { get; set; }

    // Advanced

    [JsonIgnore] // comes from registry
    public bool IsAutoStartEnabled { get; set; }

    public bool IsAutoUpdateEnabled { get; set; } = true;

    public bool IsDefaultToDayConfigurationEnabled { get; set; }

    public bool IsConfigurationSmoothingEnabled { get; set; } = true;

    public bool IsPauseWhenFullScreenEnabled { get; set; }

    public bool IsGammaPollingEnabled { get; set; }

    // Application whitelist

    public bool IsApplicationWhitelistEnabled { get; set; }

    public IReadOnlyList<ExternalApplication>? WhitelistedApplications { get; set; }

    // HotKeys

    public HotKey ToggleHotKey { get; set; }

    public HotKey IncreaseTemperatureOffsetHotKey { get; set; }

    public HotKey DecreaseTemperatureOffsetHotKey { get; set; }

    public HotKey IncreaseBrightnessOffsetHotKey { get; set; }

    public HotKey DecreaseBrightnessOffsetHotKey { get; set; }

    public HotKey ResetConfigurationOffsetHotKey { get; set; }

    // Events

    public event EventHandler? SettingsReset;

    public event EventHandler? SettingsLoaded;

    public event EventHandler? SettingsSaved;

    public override void Reset()
    {
        base.Reset();

        // Don't reset the first-time experience
        IsFirstTimeExperienceEnabled = false;
        IsUkraineSupportMessageEnabled = false;

        SettingsReset?.Invoke(this, EventArgs.Empty);
    }

    public override void Save()
    {
        // Disallow auto-start in debug mode to make things simpler
#if DEBUG
        IsAutoStartEnabled = false;
#endif

        base.Save();

        // Update values in the registry
        _extendedGammaRangeSwitch.IsSet = IsExtendedGammaRangeUnlocked;
        _autoStartSwitch.IsSet = IsAutoStartEnabled;

        SettingsSaved?.Invoke(this, EventArgs.Empty);
    }

    public override bool Load()
    {
        var wasLoaded = base.Load();

        // Get values from the registry
        IsExtendedGammaRangeUnlocked = _extendedGammaRangeSwitch.IsSet;
        IsAutoStartEnabled = _autoStartSwitch.IsSet;

        SettingsLoaded?.Invoke(this, EventArgs.Empty);

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
                "LightBulb",
                "Settings.json"
            );
        }
        // Otherwise, store them in the current directory
        else
        {
            return Path.Combine(Program.ExecutableDirPath, "Settings.json");
        }
    }
}
