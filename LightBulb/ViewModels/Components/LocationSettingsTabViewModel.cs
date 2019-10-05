using System;
using LightBulb.Internal;
using LightBulb.Models;
using LightBulb.Services;
using Stylet;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels.Components
{
    public class LocationSettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly SettingsService _settingsService;
        private readonly LocationService _locationService;

        public bool IsBusy { get; private set; }

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

        public bool IsLocationAutoDetected { get; private set; }

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
            : base(1, "Location")
        {
            _settingsService = settingsService;
            _locationService = locationService;

            // Set location query
            LocationQuery = Location?.ToString();

            // HACK: when settings change - fire property changed event for all properties in this view model
            _settingsService.Bind((sender, args) => Refresh());

            // HACK: re-calculate sunrise/sunset when location changes
            _settingsService.Bind(o => o.Location, (sender, args) =>
            {
                var location = args.NewValue;
                var instant = DateTimeOffset.Now;

                if (location != null)
                {
                    _settingsService.SunriseTime = calculationService.CalculateSunrise(location.Value, instant).TimeOfDay;
                    _settingsService.SunsetTime = calculationService.CalculateSunset(location.Value, instant).TimeOfDay;
                }
            });
        }

        public bool CanAutoDetectLocation => !IsBusy && !IsLocationAutoDetected;

        public async void AutoDetectLocation()
        {
            IsBusy = true;

            try
            {
                // Get location based on current IP
                Location = await _locationService.GetLocationAsync();
                LocationQuery = Location?.ToString();
                IsLocationAutoDetected = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool CanSetLocation => !IsBusy && !LocationQuery.IsNullOrWhiteSpace() && LocationQuery != Location?.ToString();

        public async void SetLocation()
        {
            IsBusy = true;

            try
            {
                // Try to parse location in case the query contains raw coordinates
                if (GeoLocation.TryParse(LocationQuery, out var parsedLocation))
                {
                    Location = parsedLocation;
                }
                // Otherwise search for the location online
                else
                {
                    Location = await _locationService.GetLocationAsync(LocationQuery);
                }

                LocationQuery = Location?.ToString();
                IsLocationAutoDetected = false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}