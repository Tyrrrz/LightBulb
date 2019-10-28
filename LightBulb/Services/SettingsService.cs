using System;
using LightBulb.Internal;
using LightBulb.Models;
using Tyrrrz.Settings;

namespace LightBulb.Services
{
    public class SettingsService : SettingsManager
    {
        public double NightTemperature { get; set; } = 3900.0;

        public double DayTemperature { get; set; } = 6600.0;

        public double NightBrightness { get; set; } = 0.85;

        public double DayBrightness { get; set; } = 1.0;

        // TODO: get rid of this and calculate it implicitly
        public TimeSpan TemperatureTransitionDuration { get; set; } = TimeSpan.FromMinutes(90);

        public bool IsManualSunriseSunsetEnabled { get; set; } = true;

        public TimeSpan ManualSunriseTime { get; set; } = new TimeSpan(07, 20, 00);

        public TimeSpan ManualSunsetTime { get; set; } = new TimeSpan(16, 30, 00);

        public GeoLocation? Location { get; set; }

        public bool IsDefaultToDayConfigurationEnabled { get; set; } = false;

        public bool IsGammaSmoothingEnabled { get; set; } = true;

        public bool IsPauseWhenFullScreenEnabled { get; set; }

        public HotKey ToggleHotKey { get; set; }

        public SettingsService()
        {
            var applicationDirPath = AppDomain.CurrentDomain.BaseDirectory;

            // If we have write access to application directory - store configuration file there
            if (DirectoryEx.CheckWriteAccess(applicationDirPath))
            {
                Configuration.FileName = "Settings.dat";
                Configuration.SubDirectoryPath = "";
                Configuration.StorageSpace = StorageSpace.Instance;
            }
            // Otherwise - store settings in roaming app data directory
            else
            {
                Configuration.FileName = "Settings.dat";
                Configuration.SubDirectoryPath = "LightBulb";
                Configuration.StorageSpace = StorageSpace.SyncedUserDomain;
            }

            // Ignore failures when loading/saving settings
            Configuration.ThrowIfCannotLoad = false;
            Configuration.ThrowIfCannotSave = false;
        }
    }
}