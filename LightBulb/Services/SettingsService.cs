using System;
using LightBulb.Models;
using Tyrrrz.Settings;

namespace LightBulb.Services
{
    public class SettingsService : SettingsManager
    {
        public ColorTemperature MaxTemperature { get; set; } = new ColorTemperature(6600);

        public ColorTemperature MinTemperature { get; set; } = new ColorTemperature(3900);

        public TimeSpan SunriseTime { get; set; } = new TimeSpan(07, 20, 00);

        public TimeSpan SunsetTime { get; set; } = new TimeSpan(16, 30, 00);

        public TimeSpan TemperatureTransitionDuration { get; set; } = TimeSpan.FromMinutes(90);

        public GeographicCoordinates? Location { get; set; }
    }
}