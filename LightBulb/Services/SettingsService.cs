using System;
using System.Collections.Generic;
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

// Can't use [ObservableProperty] here because System.Text.Json's source generator doesn't see
// the generated properties.
[INotifyPropertyChanged]
public partial class SettingsService() : SettingsBase(GetFilePath(), SerializerContext.Default)
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

    private bool _isFirstTimeExperienceEnabled = true;
    public bool IsFirstTimeExperienceEnabled
    {
        get => _isFirstTimeExperienceEnabled;
        set => SetProperty(ref _isFirstTimeExperienceEnabled, value);
    }

    private bool _isUkraineSupportMessageEnabled = true;
    public bool IsUkraineSupportMessageEnabled
    {
        get => _isUkraineSupportMessageEnabled;
        set => SetProperty(ref _isUkraineSupportMessageEnabled, value);
    }

    private bool _isExtendedGammaRangeUnlocked;

    [JsonIgnore] // comes from registry
    public bool IsExtendedGammaRangeUnlocked
    {
        get => _isExtendedGammaRangeUnlocked;
        set => SetProperty(ref _isExtendedGammaRangeUnlocked, value);
    }

    // General

    public double MinimumTemperature => 500;

    public double MaximumTemperature => 20_000;

    public double MinimumBrightness => 0.1;

    public double MaximumBrightness => 1;

    private ColorConfiguration _dayConfiguration = new(6600, 1);
    public ColorConfiguration DayConfiguration
    {
        get => _dayConfiguration;
        set => SetProperty(ref _dayConfiguration, value);
    }

    private ColorConfiguration _nightConfiguration = new(3900, 0.85);
    public ColorConfiguration NightConfiguration
    {
        get => _nightConfiguration;
        set => SetProperty(ref _nightConfiguration, value);
    }

    private TimeSpan _configurationTransitionDuration = TimeSpan.FromMinutes(40);
    public TimeSpan ConfigurationTransitionDuration
    {
        get => _configurationTransitionDuration;
        set => SetProperty(ref _configurationTransitionDuration, value);
    }

    private double _configurationTransitionOffset;
    public double ConfigurationTransitionOffset
    {
        get => _configurationTransitionOffset;
        set => SetProperty(ref _configurationTransitionOffset, value);
    }

    private TimeSpan _configurationSmoothingMaxDuration = TimeSpan.FromSeconds(5);
    public TimeSpan ConfigurationSmoothingMaxDuration
    {
        get => _configurationSmoothingMaxDuration;
        set => SetProperty(ref _configurationSmoothingMaxDuration, value);
    }

    // Location

    private bool _isManualSunriseSunsetEnabled = true;
    public bool IsManualSunriseSunsetEnabled
    {
        get => _isManualSunriseSunsetEnabled;
        set => SetProperty(ref _isManualSunriseSunsetEnabled, value);
    }

    private TimeOnly _manualSunrise = new(07, 20);

    [JsonPropertyName("ManualSunriseTime")]
    public TimeOnly ManualSunrise
    {
        get => _manualSunrise;
        set => SetProperty(ref _manualSunrise, value);
    }

    private TimeOnly _manualSunset = new(16, 30);

    [JsonPropertyName("ManualSunsetTime")]
    public TimeOnly ManualSunset
    {
        get => _manualSunset;
        set => SetProperty(ref _manualSunset, value);
    }

    private GeoLocation? _location;
    public GeoLocation? Location
    {
        get => _location;
        set => SetProperty(ref _location, value);
    }

    // Advanced

    private ThemeVariant _theme;
    public ThemeVariant Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    private bool _isAutoStartEnabled;

    [JsonIgnore] // comes from registry
    public bool IsAutoStartEnabled
    {
        get => _isAutoStartEnabled;
        set => SetProperty(ref _isAutoStartEnabled, value);
    }

    private bool _isAutoUpdateEnabled = true;
    public bool IsAutoUpdateEnabled
    {
        get => _isAutoUpdateEnabled;
        set => SetProperty(ref _isAutoUpdateEnabled, value);
    }

    private bool _isDefaultToDayConfigurationEnabled;
    public bool IsDefaultToDayConfigurationEnabled
    {
        get => _isDefaultToDayConfigurationEnabled;
        set => SetProperty(ref _isDefaultToDayConfigurationEnabled, value);
    }

    private bool _isConfigurationSmoothingEnabled = true;
    public bool IsConfigurationSmoothingEnabled
    {
        get => _isConfigurationSmoothingEnabled;
        set => SetProperty(ref _isConfigurationSmoothingEnabled, value);
    }

    private bool _isPauseWhenFullScreenEnabled;
    public bool IsPauseWhenFullScreenEnabled
    {
        get => _isPauseWhenFullScreenEnabled;
        set => SetProperty(ref _isPauseWhenFullScreenEnabled, value);
    }

    private bool _isGammaPollingEnabled;
    public bool IsGammaPollingEnabled
    {
        get => _isGammaPollingEnabled;
        set => SetProperty(ref _isGammaPollingEnabled, value);
    }

    // Application whitelist

    private bool _isApplicationWhitelistEnabled;
    public bool IsApplicationWhitelistEnabled
    {
        get => _isApplicationWhitelistEnabled;
        set => SetProperty(ref _isApplicationWhitelistEnabled, value);
    }

    private IReadOnlyList<ExternalApplication>? _whitelistedApplications;
    public IReadOnlyList<ExternalApplication>? WhitelistedApplications
    {
        get => _whitelistedApplications;
        set => SetProperty(ref _whitelistedApplications, value);
    }

    // HotKeys

    private HotKey _toggleHotKey;
    public HotKey ToggleHotKey
    {
        get => _toggleHotKey;
        set => SetProperty(ref _toggleHotKey, value);
    }

    private HotKey _increaseTemperatureOffsetHotKey;
    public HotKey IncreaseTemperatureOffsetHotKey
    {
        get => _increaseTemperatureOffsetHotKey;
        set => SetProperty(ref _increaseTemperatureOffsetHotKey, value);
    }

    private HotKey _decreaseTemperatureOffsetHotKey;
    public HotKey DecreaseTemperatureOffsetHotKey
    {
        get => _decreaseTemperatureOffsetHotKey;
        set => SetProperty(ref _decreaseTemperatureOffsetHotKey, value);
    }

    private HotKey _increaseBrightnessOffsetHotKey;
    public HotKey IncreaseBrightnessOffsetHotKey
    {
        get => _increaseBrightnessOffsetHotKey;
        set => SetProperty(ref _increaseBrightnessOffsetHotKey, value);
    }

    private HotKey _decreaseBrightnessOffsetHotKey;
    public HotKey DecreaseBrightnessOffsetHotKey
    {
        get => _decreaseBrightnessOffsetHotKey;
        set => SetProperty(ref _decreaseBrightnessOffsetHotKey, value);
    }

    private HotKey _resetConfigurationOffsetHotKey;
    public HotKey ResetConfigurationOffsetHotKey
    {
        get => _resetConfigurationOffsetHotKey;
        set => SetProperty(ref _resetConfigurationOffsetHotKey, value);
    }

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

public partial class SettingsService
{
    [JsonSerializable(typeof(SettingsService))]
    private partial class SerializerContext : JsonSerializerContext;
}
