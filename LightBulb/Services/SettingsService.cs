using System;
using LightBulb.Models;
using LightBulb.WindowsApi;
using Tyrrrz.Settings;

namespace LightBulb.Services
{
    public class SettingsService : SettingsManager
    {
        private readonly Timer _autoSaveTimer;

        public ColorTemperature MaxTemperature { get; set; } = new ColorTemperature(6600);

        public ColorTemperature MinTemperature { get; set; } = new ColorTemperature(3900);

        public TimeSpan SunriseTime { get; set; } = new TimeSpan(07, 20, 00);

        public TimeSpan SunsetTime { get; set; } = new TimeSpan(16, 30, 00);

        public TimeSpan TemperatureTransitionDuration { get; set; } = TimeSpan.FromMinutes(90);

        public GeographicCoordinates? Location { get; set; }

        public SettingsService()
        {
            // TODO: handle portable

            // Ignore failures when loading/saving settings
            // TODO: logging?
            Configuration.ThrowIfCannotLoad = false;
            Configuration.ThrowIfCannotSave = false;

            // Set up a timer to automatically save settings to persistent storage every X seconds
            _autoSaveTimer = new Timer(TimeSpan.FromSeconds(5), () =>
            {
                // Only save if there's anything to save
                if (!IsSaved)
                    Save();
            });
        }
    }
}