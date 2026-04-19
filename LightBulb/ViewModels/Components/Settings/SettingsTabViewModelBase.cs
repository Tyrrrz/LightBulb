using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.Framework;
using LightBulb.Localization;
using LightBulb.Services;
using PowerKit;
using PowerKit.Extensions;

namespace LightBulb.ViewModels.Components.Settings;

public abstract partial class SettingsTabViewModelBase : ViewModelBase
{
    private readonly IDisposable _eventSubscription;

    protected SettingsTabViewModelBase(
        SettingsService settingsService,
        LocalizationManager localizationManager,
        int order
    )
    {
        SettingsService = settingsService;
        LocalizationManager = localizationManager;
        Order = order;

        _eventSubscription = Disposable.Merge(
            // Implementing classes will bind to settings properties through
            // their own properties, so make sure they stay in sync.
            // This is a bit overkill as it triggers a lot of unnecessary events,
            // but it's a simple and reliable solution.
            SettingsService.WatchAllProperties(OnAllPropertiesChanged),
            localizationManager.WatchProperty(o => o.Language, _ => OnAllPropertiesChanged())
        );
    }

    protected SettingsService SettingsService { get; }

    public LocalizationManager LocalizationManager { get; }

    [ObservableProperty]
    public partial bool IsActive { get; set; }

    public int Order { get; }

    public abstract string DisplayName { get; }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _eventSubscription.Dispose();

        base.Dispose(disposing);
    }
}
