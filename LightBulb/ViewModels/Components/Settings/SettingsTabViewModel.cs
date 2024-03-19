using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.Services;
using LightBulb.ViewModels.Framework;

namespace LightBulb.ViewModels.Components.Settings;

public abstract partial class SettingsTabViewModel : ViewModelBase, ISettingsTabViewModel
{
    [ObservableProperty]
    private bool _isActive;

    protected SettingsService SettingsService { get; }

    public int Order { get; }

    public string DisplayName { get; }

    protected SettingsTabViewModel(SettingsService settingsService, int order, string displayName)
    {
        SettingsService = settingsService;
        Order = order;
        DisplayName = displayName;

        SettingsService.SettingsReset += (_, _) => OnAllPropertiesChanged();
        SettingsService.SettingsLoaded += (_, _) => OnAllPropertiesChanged();
        SettingsService.SettingsSaved += (_, _) => OnAllPropertiesChanged();
    }
}
