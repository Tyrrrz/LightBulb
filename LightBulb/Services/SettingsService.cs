using System;
using System.Collections.Generic;
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

        public bool IsDefaultToDayConfigurationEnabled { get; set; } = false;

        public bool IsConfigurationSmoothingEnabled { get; set; } = true;

        public bool IsPauseWhenFullScreenEnabled { get; set; } = false;

        public bool IsGammaPollingEnabled { get; set; } = false;

        // Application whitelist

        public bool IsApplicationWhitelistEnabled { get; set; } = false;

        public IReadOnlyList<ExternalApplication>? WhitelistedApplications { get; set; }

        // HotKeys

        public HotKey ToggleHotKey { get; set; }

        // Events

        public event EventHandler? SettingsReset;

        public event EventHandler? SettingsSaved;

        public SettingsService()
        {
            // If we have write access to application directory - store configuration file there
            if (DirectoryEx.CheckWriteAccess(App.ExecutableDirPath))
            {
                Configuration.SubDirectoryPath = "";
                Configuration.StorageSpace = StorageSpace.Instance;
            }
            // Otherwise - store settings in roaming app data directory
            else
            {
                Configuration.SubDirectoryPath = "LightBulb";
                Configuration.StorageSpace = StorageSpace.SyncedUserDomain;
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
            base.Save();

            // Update values in the registry
            _extendedGammaRangeSwitch.SetEnabled(IsExtendedGammaRangeUnlocked);
            _autoStartSwitch.SetEnabled(IsAutoStartEnabled);

            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }
    }

    public partial class SettingsService
    {
        // Converter to handle time of day serialization
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