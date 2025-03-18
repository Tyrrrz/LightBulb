using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Core;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;

namespace LightBulb.ViewModels.Components.Settings;

public partial class LocationSettingsTabViewModel : SettingsTabViewModelBase
{
    private readonly DisposableCollector _eventRoot = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AutoResolveLocationCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResolveLocationCommand))]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial bool IsLocationError { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResolveLocationCommand))]
    public partial string? LocationQuery { get; set; }

    public LocationSettingsTabViewModel(SettingsService settingsService)
        : base(settingsService, 1, "Location")
    {
        _eventRoot.Add(
            this.WatchProperty(o => o.Location, () => LocationQuery = Location?.ToString(), true)
        );
    }

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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _eventRoot.Dispose();

        base.Dispose(disposing);
    }
}
