using System;
using LightBulb.Models;
using LightBulb.Timers;
using Tyrrrz.Settings;

namespace LightBulb.Services
{
    public class SettingsService : SettingsManager, IDisposable
    {
        private readonly AutoResetTimer _autoSaveTimer;

        public ColorTemperature MaxTemperature { get; set; } = new ColorTemperature(6600);

        public ColorTemperature MinTemperature { get; set; } = new ColorTemperature(3900);

        public TimeSpan SunriseTime { get; set; } = new TimeSpan(07, 20, 00);

        public TimeSpan SunsetTime { get; set; } = new TimeSpan(16, 30, 00);

        public TimeSpan TemperatureTransitionDuration { get; set; } = TimeSpan.FromMinutes(90);

        public GeoLocation? Location { get; set; }

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

            // Set up a timer to automatically save settings to persistent storage every X seconds
            _autoSaveTimer = new AutoResetTimer(Save).Start(TimeSpan.FromSeconds(5));
        }

        public void Dispose() => _autoSaveTimer.Dispose();
    }
}