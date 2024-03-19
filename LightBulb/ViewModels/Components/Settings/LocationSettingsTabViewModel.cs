using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Core;
using LightBulb.Services;
using LightBulb.Utils.Extensions;

namespace LightBulb.ViewModels.Components.Settings;

public partial class LocationSettingsTabViewModel : SettingsTabViewModel, IDisposable
{
    private readonly IDisposable _eventPool;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AutoResolveLocationCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResolveLocationCommand))]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isLocationError;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResolveLocationCommand))]
    private string? _locationQuery;

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

    public LocationSettingsTabViewModel(SettingsService settingsService)
        : base(settingsService, 1, "Location")
    {
        // Watch property changes on other objects
        _eventPool = new[]
        {
            // Update the location query when the actual location changes
            settingsService.WatchProperty(
                o => o.Location,
                () => LocationQuery = Location?.ToString()
            )
        }.Aggregate();
    }

    private bool CanAutoResolveLocation() => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanAutoResolveLocation))]
    private async Task AutoResolveLocationAsync()
    {
        if (IsBusy)
            return;

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

    private bool CanResolveLocation() =>
        !IsBusy
        && !string.IsNullOrWhiteSpace(LocationQuery)
        && LocationQuery != Location?.ToString();

    [RelayCommand(CanExecute = nameof(CanResolveLocation))]
    private async Task ResolveLocationAsync()
    {
        if (
            IsBusy
            || string.IsNullOrWhiteSpace(LocationQuery)
            || LocationQuery == Location?.ToString()
        )
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

    public void Dispose() => _eventPool.Dispose();
}
