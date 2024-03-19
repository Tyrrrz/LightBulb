using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components.Settings;

public abstract partial class SettingsTabViewModel : ObservableObject, ISettingsTabViewModel
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

        SettingsService.SettingsReset += (_, _) => Refresh();
        SettingsService.SettingsLoaded += (_, _) => Refresh();
        SettingsService.SettingsSaved += (_, _) => Refresh();
    }
}
