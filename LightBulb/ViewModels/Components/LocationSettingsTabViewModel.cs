using System;
using LightBulb.Models;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components
{
    public class LocationSettingsTabViewModel : SettingsTabViewModel
    {
        private readonly SettingsService _settingsService;

        public TimeSpan SunriseTime
        {
            get => _settingsService.SunriseTime;
            set => _settingsService.SunriseTime = value;
        }

        public TimeSpan SunsetTime
        {
            get => _settingsService.SunsetTime;
            set => _settingsService.SunsetTime = value;
        }

        public GeoLocation? Location
        {
            get => _settingsService.Location;
            set => _settingsService.Location = value;
        }

        public bool IsInternetSyncEnabled
        {
            get => _settingsService.IsInternetSyncEnabled;
            set => _settingsService.IsInternetSyncEnabled = value;
        }

        public LocationSettingsTabViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;

            // Set display name
            DisplayName = "Location settings";

            // HACK: when settings change - fire property changed event for all properties in this view model
            _settingsService.PropertyChanged += (sender, args) => NotifyOfPropertyChange(null);
        }
    }
}