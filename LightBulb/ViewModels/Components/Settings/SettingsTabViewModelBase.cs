using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.Framework;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components.Settings;

public abstract partial class SettingsTabViewModelBase : ViewModelBase
{
    [ObservableProperty]
    private bool _isActive;

    protected SettingsService SettingsService { get; }

    public int Order { get; }

    public string DisplayName { get; }

    protected SettingsTabViewModelBase(
        SettingsService settingsService,
        int order,
        string displayName
    )
    {
        SettingsService = settingsService;
        Order = order;
        DisplayName = displayName;

        SettingsService.SettingsReset += (_, _) => OnAllPropertiesChanged();
        SettingsService.SettingsLoaded += (_, _) => OnAllPropertiesChanged();
        SettingsService.SettingsSaved += (_, _) => OnAllPropertiesChanged();
    }
}
