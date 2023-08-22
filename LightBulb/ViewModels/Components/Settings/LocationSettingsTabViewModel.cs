using System;
using System.Threading.Tasks;
using LightBulb.Core;
using LightBulb.Services;
using Stylet;

namespace LightBulb.ViewModels.Components.Settings;

public class LocationSettingsTabViewModel : SettingsTabViewModelBase
{
    public bool IsBusy { get; private set; }

    public bool IsManualSunriseSunsetEnabled
    {
        get => SettingsService.IsManualSunriseSunsetEnabled;
        set => SettingsService.IsManualSunriseSunsetEnabled = value;
    }

    public TimeOnly ManualSunrise
    {
        get => SettingsService.ManualSunrise;
        set => SettingsService.ManualSunrise = value;
    }

    public TimeOnly ManualSunset
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
        // Update the location query when the actual location changes
        settingsService.BindAndInvoke(
            o => o.Location,
            (_, _) => LocationQuery = Location?.ToString()
        );
    }

    public bool CanAutoDetectLocationAsync => !IsBusy;

    public async Task AutoDetectLocationAsync()
    {
        IsBusy = true;
        IsLocationError = false;

        try
        {
            Location = await GeoLocation.GetCurrentAsync();
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

    public bool CanSetLocationAsync =>
        !IsBusy
        && !string.IsNullOrWhiteSpace(LocationQuery)
        && LocationQuery != Location?.ToString();

    public async Task SetLocationAsync()
    {
        if (string.IsNullOrWhiteSpace(LocationQuery))
            return;

        IsBusy = true;
        IsLocationError = false;

        try
        {
            Location =
                GeoLocation.TryParse(LocationQuery) ?? await GeoLocation.SearchAsync(LocationQuery);
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
