using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.Core;
using LightBulb.Models;
using LightBulb.PlatformInterop;
using LightBulb.Utils;
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
            Program.Name,
            $"\"{Program.ExecutableFilePath}\" {StartOptions.IsInitiallyHiddenArgument}"
        );

    [ObservableProperty]
    private bool _isFirstTimeExperienceEnabled = true;

    [ObservableProperty]
    private bool _isUkraineSupportMessageEnabled = true;

    [ObservableProperty]
    [property: JsonIgnore] // comes from registry
    private bool _isExtendedGammaRangeUnlocked;

    // General

    public double MinimumTemperature => 500;

    public double MaximumTemperature => 20_000;

    public double MinimumBrightness => 0.1;

    public double MaximumBrightness => 1;

    [ObservableProperty]
    private ColorConfiguration _nightConfiguration = new(3900, 0.85);

    [ObservableProperty]
    private ColorConfiguration _dayConfiguration = new(6600, 1);

    [ObservableProperty]
    private TimeSpan _configurationTransitionDuration = TimeSpan.FromMinutes(40);

    [ObservableProperty]
    private double _configurationTransitionOffset;

    [ObservableProperty]
    private TimeSpan _maxSettingsTransitionDuration = TimeSpan.FromSeconds(1);

    // Location

    [ObservableProperty]
    private bool _isManualSunriseSunsetEnabled = true;

    [ObservableProperty]
    [property: JsonPropertyName("ManualSunriseTime")]
    private TimeOnly _manualSunrise = new(07, 20);

    [ObservableProperty]
    [property: JsonPropertyName("ManualSunsetTime")]
    private TimeOnly _manualSunset = new(16, 30);

    [ObservableProperty]
    private GeoLocation? _location;

    // Advanced

    [ObservableProperty]
    [property: JsonIgnore] // comes from registry
    private bool _isAutoStartEnabled;

    [ObservableProperty]
    private bool _isAutoUpdateEnabled = true;

    [ObservableProperty]
    private bool _isDefaultToDayConfigurationEnabled;

    [ObservableProperty]
    private bool _isConfigurationSmoothingEnabled = true;

    [ObservableProperty]
    private bool _isPauseWhenFullScreenEnabled;

    [ObservableProperty]
    private bool _isGammaPollingEnabled;

    // Application whitelist

    [ObservableProperty]
    private bool _isApplicationWhitelistEnabled;

    [ObservableProperty]
    private IReadOnlyList<ExternalApplication>? _whitelistedApplications;

    // HotKeys

    [ObservableProperty]
    private HotKey _toggleHotKey;

    [ObservableProperty]
    private HotKey _increaseTemperatureOffsetHotKey;

    [ObservableProperty]
    private HotKey _decreaseTemperatureOffsetHotKey;

    [ObservableProperty]
    private HotKey _increaseBrightnessOffsetHotKey;

    [ObservableProperty]
    private HotKey _decreaseBrightnessOffsetHotKey;

    [ObservableProperty]
    private HotKey _resetConfigurationOffsetHotKey;

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
        _extendedGammaRangeSwitch.IsSet = IsExtendedGammaRangeUnlocked;
        _autoStartSwitch.IsSet = IsAutoStartEnabled;

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
        else
        {
            return Path.Combine(Program.ExecutableDirPath, "Settings.json");
        }
    }
}
