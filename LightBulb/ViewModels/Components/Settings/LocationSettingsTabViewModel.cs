using LightBulb.Domain;
using LightBulb.Services;
using Stylet;

namespace LightBulb.ViewModels.Components.Settings
{
    public class LocationSettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly GeoLocationProvider _locationProvider = new GeoLocationProvider();

        public bool IsBusy { get; private set; }

        public bool IsManualSunriseSunsetEnabled
        {
            get => SettingsService.IsManualSunriseSunsetEnabled;
            set => SettingsService.IsManualSunriseSunsetEnabled = value;
        }

        public TimeOfDay ManualSunrise
        {
            get => SettingsService.ManualSunrise;
            set => SettingsService.ManualSunrise = value;
        }

        public TimeOfDay ManualSunset
        {
            get => SettingsService.ManualSunset;
            set => SettingsService.ManualSunset = value;
        }

        public GeoLocation? Location
        {
            get => SettingsService.Location;
            set => SettingsService.Location = value;
        }

        public bool IsLocationError { get; private set; }

        public string? LocationQuery { get; set; }

        public LocationSettingsTabViewModel(SettingsService settingsService)
            : base(settingsService, 1, "Location")
        {
            // Bind string representation of location to the actual value
            settingsService.BindAndInvoke(o => o.Location, (sender, args) => LocationQuery = Location?.ToString());
        }

        public bool CanAutoDetectLocation => !IsBusy;

        public async void AutoDetectLocation()
        {
            IsBusy = true;
            IsLocationError = false;

            try
            {
                Location = await _locationProvider.GetLocationAsync();
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

        public bool CanSetLocation =>
            !IsBusy &&
            !string.IsNullOrWhiteSpace(LocationQuery) &&
            LocationQuery != Location?.ToString();

        public async void SetLocation()
        {
            if (string.IsNullOrWhiteSpace(LocationQuery))
                return;

            IsBusy = true;
            IsLocationError = false;

            try
            {
                Location =
                    GeoLocation.TryParse(LocationQuery) ??
                    await _locationProvider.GetLocationAsync(LocationQuery);
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