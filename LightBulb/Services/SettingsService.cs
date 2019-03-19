using System;
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
        public bool IsInternetSyncEnabled { get; set; }

        public SettingsService()
        {
            // TODO: handle non-portable

            Configuration.FileName = "Config.dat";
            Configuration.SubDirectoryPath = "";
            Configuration.StorageSpace = StorageSpace.Instance;

            // Ignore failures when loading/saving settings
            // TODO: logging?
            Configuration.ThrowIfCannotLoad = false;
            Configuration.ThrowIfCannotSave = false;
        }
    }
}