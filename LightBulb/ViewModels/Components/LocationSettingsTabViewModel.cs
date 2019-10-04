using System;
using LightBulb.Internal;
using LightBulb.Models;
using LightBulb.Services;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels.Components
{
    public class LocationSettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly SettingsService _settingsService;
        private readonly LocationService _locationService;
        private readonly CalculationService _calculationService;

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

        public string LocationQuery { get; set; }

        public GeoLocation? Location
        {
            get => _settingsService.Location;
            set => _settingsService.Location = value;
        }

        public bool IsManualSunriseSunset
        {
            get => _settingsService.IsManualSunriseSunset;
            set => _settingsService.IsManualSunriseSunset = value;
        }

        public LocationSettingsTabViewModel(SettingsService settingsService, LocationService locationService,
            CalculationService calculationService)
        {
            _settingsService = settingsService;
            _locationService = locationService;
            _calculationService = calculationService;

            // Set display name
            DisplayName = "Location";

            // Set order
            Order = 1;

            // Set location query
            LocationQuery = Location?.ToString();

            // HACK: when settings change - fire property changed event for all properties in this view model
            _settingsService.Bind((sender, args) => NotifyOfPropertyChange(null));
        }

        private void SetLocation(GeoLocation location)
        {
            // Set location
            Location = location;
            LocationQuery = location.ToString();

            // Update sunrise and sunset times
            var instant = DateTimeOffset.Now;
            _settingsService.SunriseTime = _calculationService.CalculateSunrise(location, instant).TimeOfDay;
            _settingsService.SunsetTime = _calculationService.CalculateSunset(location, instant).TimeOfDay;
        }

        public async void ResolveCurrentLocation()
        {
            // Get location based on current IP
            var location = await _locationService.GetLocationAsync();
            SetLocation(location);
        }

        public bool CanProcessLocationQuery => !LocationQuery.IsNullOrWhiteSpace();

        public async void ResolveLocation()
        {
            // Try to parse location in case the query contains raw coordinates
            if (GeoLocation.TryParse(LocationQuery, out var parsedLocation))
            {
                SetLocation(parsedLocation);
            }
            // Otherwise search for the location online
            else
            {
                var location = await _locationService.GetLocationAsync(LocationQuery);
                SetLocation(location);
            }
        }
    }
}