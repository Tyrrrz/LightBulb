using System;
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

        public bool IsManualSunriseSunsetEnabled
        {
            get => _settingsService.IsManualSunriseSunsetEnabled;
            set => _settingsService.IsManualSunriseSunsetEnabled = value;
        }

        public TimeSpan ManualSunriseTime
        {
            get => _settingsService.ManualSunriseTime;
            set => _settingsService.ManualSunriseTime = value.Clamp(TimeSpan.Zero, new TimeSpan(23, 59, 59));
        }

        public TimeSpan ManualSunsetTime
        {
            get => _settingsService.ManualSunsetTime;
            set => _settingsService.ManualSunsetTime = value.Clamp(TimeSpan.Zero, new TimeSpan(23, 59, 59));
        }

        public GeoLocation? Location
        {
            get => _settingsService.Location;
            set => _settingsService.Location = value;
        }

        public bool IsLocationAutoDetected { get; private set; }

        public bool IsLocationError { get; private set; }

        public string LocationQuery { get; set; }

        public LocationSettingsTabViewModel(SettingsService settingsService, LocationService locationService)
            : base(1, "Location")
        {
            _settingsService = settingsService;
            _locationService = locationService;

            // Bind location query to location
            _settingsService.BindAndInvoke(o => o.Location, (sender, args) => LocationQuery = Location?.ToString());
        }

        public bool CanAutoDetectLocation => !IsBusy && !IsLocationAutoDetected;

        public async void AutoDetectLocation()
        {
            IsBusy = true;
            IsLocationError = false;

            try
            {
                // Get location based on current IP
                Location = await _locationService.GetLocationAsync();
                IsLocationAutoDetected = true;
            }
            catch
            {
                IsLocationError = true;
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
            IsLocationError = false;

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

                IsLocationAutoDetected = false;
            }
            catch
            {
                IsLocationError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}