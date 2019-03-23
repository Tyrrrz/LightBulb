using System;
using System.Linq;
using LightBulb.Models;
using Tyrrrz.Settings;

namespace LightBulb.Services
{
    public class SettingsService : SettingsManager
    {
        public ColorTemperature MaxTemperature { get; set; } = new ColorTemperature(6600);

        public ColorTemperature MinTemperature { get; set; } = new ColorTemperature(3900);

        public TimeSpan TemperatureTransitionDuration { get; set; } = TimeSpan.FromMinutes(90);

        public TimeSpan SunriseTime { get; set; } = new TimeSpan(07, 20, 00);

        public TimeSpan SunsetTime { get; set; } = new TimeSpan(16, 30, 00);

        public GeoLocation? Location { get; set; }

        // TODO: rename
        public bool IsInternetSyncEnabled { get; set; } = true;

        public bool IsGammaPollingEnabled { get; set; } = true;

        public bool IsGammaSmoothingEnabled { get; set; } = true;

        public bool IsPauseWhenFullScreenEnabled { get; set; }

        public SettingsService()
        {
            // Determine if application is running as portable
            var isPortable = Environment.GetCommandLineArgs().Contains("--portable");

            // If portable - store settings in the same directory as the executable
            if (isPortable)
            {
                Configuration.FileName = "Config.dat";
                Configuration.SubDirectoryPath = "";
                Configuration.StorageSpace = StorageSpace.Instance;
            }
            // If not portable - store settings in roaming app data directory
            else
            {
                Configuration.FileName = "Config.dat";
                Configuration.SubDirectoryPath = "LightBulb";
                Configuration.StorageSpace = StorageSpace.SyncedUserDomain;
            }

            // Ignore failures when loading/saving settings
            Configuration.ThrowIfCannotLoad = false;
            Configuration.ThrowIfCannotSave = false;
        }
    }
}