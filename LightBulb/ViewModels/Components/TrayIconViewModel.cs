using System;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Framework;
using LightBulb.Localization;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;

namespace LightBulb.ViewModels.Components;

public partial class TrayIconViewModel : ViewModelBase
{
    private readonly ViewModelManager _viewModelManager;
    private readonly DialogManager _dialogManager;

    private readonly DisposableCollector _eventRoot = new();

    public LocalizationManager LocalizationManager { get; }

    public DashboardViewModel Dashboard { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowHideMenuItemHeader))]
    public partial bool IsWindowVisible { get; set; }

    public string ShowHideMenuItemHeader =>
        IsWindowVisible
            ? LocalizationManager.TrayHideMenuItem
            : LocalizationManager.TrayShowMenuItem;

    public string ToolTipText =>
        Program.Name
        + Environment.NewLine
        + (
            Dashboard.IsActive
                ? Dashboard.CurrentConfiguration.Temperature.ToString("F0")
                    + " / "
                    + Dashboard.CurrentConfiguration.Brightness.ToString("P0")
                : LocalizationManager.TrayTooltipDisabled
        );

    public string ToggleMenuItemHeader =>
        Dashboard.IsEnabled
            ? LocalizationManager.TrayDisableOnlyMenuItem
            : LocalizationManager.TrayEnableMenuItem;

    public TrayIconViewModel(
        DashboardViewModel dashboard,
        ViewModelManager viewModelManager,
        DialogManager dialogManager,
        LocalizationManager localizationManager
    )
    {
        Dashboard = dashboard;
        _viewModelManager = viewModelManager;
        _dialogManager = dialogManager;
        LocalizationManager = localizationManager;

        _eventRoot.Add(
            localizationManager.WatchProperty(
                o => o.Language,
                _ =>
                {
                    OnPropertyChanged(nameof(ShowHideMenuItemHeader));
                    OnPropertyChanged(nameof(ToolTipText));
                    OnPropertyChanged(nameof(ToggleMenuItemHeader));
                }
            )
        );

        _eventRoot.Add(
            dashboard.WatchProperty(
                o => o.IsEnabled,
                _ =>
                {
                    OnPropertyChanged(nameof(ToolTipText));
                    OnPropertyChanged(nameof(ToggleMenuItemHeader));
                }
            )
        );

        _eventRoot.Add(
            dashboard.WatchProperty(o => o.IsPaused, _ => OnPropertyChanged(nameof(ToolTipText)))
        );

        _eventRoot.Add(
            dashboard.WatchProperty(
                o => o.IsCyclePreviewEnabled,
                _ => OnPropertyChanged(nameof(ToolTipText))
            )
        );

        _eventRoot.Add(
            dashboard.WatchProperty(
                o => o.CurrentConfiguration,
                _ => OnPropertyChanged(nameof(ToolTipText))
            )
        );
    }

    [RelayCommand]
    private async Task ShowSettingsFromTrayAsync()
    {
        var window = App.Current?.ShowMainWindow();
        if (window is null)
            return;

        // Wait until the window is loaded to avoid potential issues
        // with showing a dialog too early in the lifecycle.
        try
        {
            await window.WaitUntilLoadedAsync();
        }
        catch (OperationCanceledException)
        {
            return;
        }

        await _dialogManager.ShowDialogAsync(_viewModelManager.GetSettingsViewModel());
    }

    [RelayCommand]
    private void ToggleWindow() => App.Current?.ToggleMainWindow();

    [RelayCommand]
    private void Exit()
    {
        if (Application.Current?.ApplicationLifetime?.TryShutdown() != true)
            Environment.Exit(0);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _eventRoot.Dispose();
        }

        base.Dispose(disposing);
    }
}
