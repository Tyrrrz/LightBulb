using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.Framework;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;

namespace LightBulb.ViewModels.Components.Settings;

public abstract partial class SettingsTabViewModelBase : ViewModelBase
{
    private readonly DisposableCollector _eventRoot = new();

    [ObservableProperty]
    public partial bool IsActive { get; set; }

    protected SettingsTabViewModelBase(
        SettingsService settingsService,
        int order,
        string displayName
    )
    {
        SettingsService = settingsService;
        Order = order;
        DisplayName = displayName;

        _eventRoot.Add(
            // Implementing classes will bind to settings properties through
            // their own properties, so make sure they stay in sync.
            // This is a bit overkill as it triggers a lot of unnecessary events,
            // but it's a simple and reliable solution.
            SettingsService.WatchAllProperties(OnAllPropertiesChanged)
        );
    }

    protected SettingsService SettingsService { get; }

    public int Order { get; }

    public string DisplayName { get; }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _eventRoot.Dispose();

        base.Dispose(disposing);
    }
}
