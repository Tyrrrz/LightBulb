using System;
using LightBulb.Internal;
using LightBulb.Models;
using Tyrrrz.Settings;

namespace LightBulb.Services
{
    public class SettingsService : SettingsManager
    {
        public ColorTemperature MaxTemperature { get; set; } = new ColorTemperature(6600);

        public ColorTemperature MinTemperature { get; set; } = new ColorTemperature(3900);

        // TODO: get rid of this and calculate it implicitly
        public TimeSpan TemperatureTransitionDuration { get; set; } = TimeSpan.FromMinutes(90);

        public TimeSpan SunriseTime { get; set; } = new TimeSpan(07, 20, 00);

        public TimeSpan SunsetTime { get; set; } = new TimeSpan(16, 30, 00);

        public GeoLocation? Location { get; set; }

        public bool IsManualSunriseSunset { get; set; } = true;

        public bool IsGammaPollingEnabled { get; set; } = true;

        public bool IsGammaSmoothingEnabled { get; set; } = true;

        public bool IsPauseWhenFullScreenEnabled { get; set; }

        public HotKey ToggleHotKey { get; set; }

        public HotKey ToggleGammaPollingHotKey { get; set; }

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

        public void SaveIfNeeded()
        {
            if (!IsSaved)
                Save();
        }
    }
}