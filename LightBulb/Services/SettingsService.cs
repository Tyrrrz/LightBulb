using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LightBulb.Core;
using LightBulb.Models;
using LightBulb.Utils;
using LightBulb.WindowsApi;
using Newtonsoft.Json;
using Tyrrrz.Settings;

namespace LightBulb.Services
{
    public partial class SettingsService : SettingsManager
    {
        private readonly RegistrySwitch _extendedGammaRangeSwitch = new(
            "HKLM\\Software\\Microsoft\\Windows NT\\CurrentVersion\\ICM",
            "GdiICMGammaRange",
            256
        );

        private readonly RegistrySwitch _autoStartSwitch = new(
            "HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Run",
            App.Name,
            $"\"{App.ExecutableFilePath}\" {App.HiddenOnLaunchArgument}"
        );

        public bool IsFirstTimeExperienceEnabled { get; set; } = true;

        [Ignore] // comes from registry
        public bool IsExtendedGammaRangeUnlocked { get; set; }

        // General

        [Ignore] // not configurable
        public double MinimumTemperature => 500;

        [Ignore] // not configurable
        public double MaximumTemperature => 20_000;

        [Ignore] // not configurable
        public double MinimumBrightness => 0.1;

        [Ignore] // not configurable
        public double MaximumBrightness => 1;

        public ColorConfiguration NightConfiguration { get; set; } = new(3900, 0.85);

        public ColorConfiguration DayConfiguration { get; set; } = new(6600, 1);

        public TimeSpan ConfigurationTransitionDuration { get; set; } = TimeSpan.FromMinutes(40);

        public double ConfigurationTransitionOffset { get; set; }

        // Location

        public bool IsManualSunriseSunsetEnabled { get; set; } = true;

        [JsonProperty("ManualSunriseTime"), JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly ManualSunrise { get; set; } = new(07, 20);

        [JsonProperty("ManualSunsetTime"), JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly ManualSunset { get; set; } = new(16, 30);

        public GeoLocation? Location { get; set; }

        // Advanced

        [Ignore] // comes from registry
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

        public SettingsService()
        {
            var installerMarker = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory(),
                ".installed"
            );

            var isInstalled = File.Exists(installerMarker);

            // Prefer storing settings in appdata when installed or when current directory is write-protected
            if (isInstalled || !DirectoryEx.CheckWriteAccess(App.ExecutableDirPath))
            {
                Configuration.StorageSpace = StorageSpace.SyncedUserDomain;
                Configuration.SubDirectoryPath = "LightBulb";
            }
            // Otherwise, store them in the current directory
            else
            {
                Configuration.StorageSpace = StorageSpace.Instance;
                Configuration.SubDirectoryPath = "";
            }

            Configuration.FileName = "Settings.json";
            Configuration.ThrowIfCannotLoad = false;
            Configuration.ThrowIfCannotSave = true;
        }

        public override void Reset()
        {
            base.Reset();
            SettingsReset?.Invoke(this, EventArgs.Empty);
        }

        public override void Load()
        {
            base.Load();

            // Get the actual values from registry because it may be out of sync with saved settings
            IsExtendedGammaRangeUnlocked = _extendedGammaRangeSwitch.IsSet;
            IsAutoStartEnabled = _autoStartSwitch.IsSet;

            SettingsLoaded?.Invoke(this, EventArgs.Empty);
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
    }

    public partial class SettingsService
    {
        // Converter to handle (de-)serialization of TimeOnly
        private class TimeOnlyJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(TimeOnly);

            public override void WriteJson(
                JsonWriter writer,
                object value,
                JsonSerializer serializer)
            {
                if (value is TimeOnly timeOfDay)
                    writer.WriteValue(timeOfDay.ToTimeSpan());
            }

            public override object ReadJson(
                JsonReader reader,
                Type objectType,
                object existingValue,
                JsonSerializer serializer) =>
                TimeOnly.TryParse((string) reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
                    ? result
                    : default;
        }
    }
}