﻿using System;
using System.Collections.Generic;
using System.IO;
using LightBulb.Domain;
using LightBulb.Internal;
using LightBulb.Models;
using LightBulb.WindowsApi;
using Newtonsoft.Json;
using Tyrrrz.Settings;

namespace LightBulb.Services
{
    public partial class SettingsService : SettingsManager
    {
        private readonly RegistrySwitch _extendedGammaRangeSwitch = new RegistrySwitch(
            "HKLM\\Software\\Microsoft\\Windows NT\\CurrentVersion\\ICM",
            "GdiICMGammaRange",
            256
        );

        private readonly RegistrySwitch _autoStartSwitch = new RegistrySwitch(
            "HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Run",
            App.Name,
            $"\"{App.ExecutableFilePath}\" {App.HiddenOnLaunchArgument}"
        );

        public bool IsFirstTimeExperienceEnabled { get; set; } = true;

        [Ignore] // comes from registry
        public bool IsExtendedGammaRangeUnlocked { get; set; }

        // General

        public ColorConfiguration NightConfiguration { get; set; } = new ColorConfiguration(3900, 0.85);

        public ColorConfiguration DayConfiguration { get; set; } = new ColorConfiguration(6600, 1);

        public TimeSpan ConfigurationTransitionDuration { get; set; } = TimeSpan.FromMinutes(30);

        // Location

        public bool IsManualSunriseSunsetEnabled { get; set; } = true;

        [JsonProperty("ManualSunriseTime"), JsonConverter(typeof(TimeOfDayJsonConverter))]
        public TimeOfDay ManualSunrise { get; set; } = new TimeOfDay(07, 20);

        [JsonProperty("ManualSunsetTime"), JsonConverter(typeof(TimeOfDayJsonConverter))]
        public TimeOfDay ManualSunset { get; set; } = new TimeOfDay(16, 30);

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

        public HotKey ResetOffsetHotKey { get; set; }

        // Events

        public event EventHandler? SettingsReset;

        public event EventHandler? SettingsSaved;

        public SettingsService()
        {
            var installerMarker = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory(), ".installed");
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
            IsExtendedGammaRangeUnlocked = _extendedGammaRangeSwitch.IsEnabled();
            IsAutoStartEnabled = _autoStartSwitch.IsEnabled();
        }

        public override void Save()
        {
            // Disallow auto-start in debug mode to make things simpler
#if DEBUG
            IsAutoStartEnabled = false;
#endif

            base.Save();

            // Update values in the registry
            _extendedGammaRangeSwitch.SetEnabled(IsExtendedGammaRangeUnlocked);
            _autoStartSwitch.SetEnabled(IsAutoStartEnabled);

            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }
    }

    public partial class SettingsService
    {
        // Converter to handle (de-)serialization of TimeOfDay
        private class TimeOfDayJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(TimeOfDay);

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value is TimeOfDay timeOfDay)
                    writer.WriteValue(timeOfDay.AsTimeSpan());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var raw = (string) reader.Value;
                return TimeOfDay.TryParse(raw) ?? default;
            }
        }
    }
}