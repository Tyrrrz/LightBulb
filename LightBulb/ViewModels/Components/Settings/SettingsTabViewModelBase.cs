using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.Framework;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;

namespace LightBulb.ViewModels.Components.Settings;

public abstract partial class SettingsTabViewModelBase : ViewModelBase
{
    private readonly DisposablePool _disposablePool = new();

    [ObservableProperty]
    private bool _isActive;

    protected SettingsTabViewModelBase(
        SettingsService settingsService,
        int order,
        string displayName
    )
    {
        SettingsService = settingsService;
        Order = order;
        DisplayName = displayName;

        _disposablePool.Add(
            // Implementing classes will bind to settings properties through
            // their own properties, so make sure they stay in sync.
            SettingsService.WatchAllProperties(OnAllPropertiesChanged)
        );
    }

    protected SettingsService SettingsService { get; }

    public int Order { get; }

    public string DisplayName { get; }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposablePool.Dispose();
        }

        base.Dispose(disposing);
    }
}
