using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Services;
using LightBulb.ViewModels.Components.Settings;
using LightBulb.ViewModels.Framework;

namespace LightBulb.ViewModels.Dialogs;

public partial class SettingsViewModel : DialogViewModelBase
{
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private SettingsTabViewModelBase? _activeTab;

    public IReadOnlyList<SettingsTabViewModelBase> Tabs { get; }

    public SettingsViewModel(
        SettingsService settingsService,
        IEnumerable<SettingsTabViewModelBase> tabs
    )
    {
        _settingsService = settingsService;
        Tabs = tabs.OrderBy(t => t.Order).ToArray();

        // Pre-select first tab
        var firstTab = Tabs.FirstOrDefault();
        if (firstTab is not null)
            ActivateTab(firstTab);
    }

    [RelayCommand]
    private void ActivateTab(SettingsTabViewModelBase settingsTab)
    {
        // Deactivate the previously selected tab
        if (ActiveTab is not null)
            ActiveTab.IsActive = false;

        ActiveTab = settingsTab;
        settingsTab.IsActive = true;
    }

    public void ActivateTab<T>()
        where T : SettingsTabViewModelBase
    {
        var tab = Tabs.OfType<T>().FirstOrDefault();
        if (tab is not null)
            ActivateTab(tab);
    }

    [RelayCommand]
    private void Reset() => _settingsService.Reset();

    [RelayCommand]
    private void Save()
    {
        _settingsService.Save();
        Close(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        _settingsService.Load();
        Close(false);
    }
}
