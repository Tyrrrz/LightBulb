using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Framework;
using LightBulb.Localization;
using LightBulb.PlatformInterop;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;
using Process = System.Diagnostics.Process;

namespace LightBulb.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ViewModelManager _viewModelManager;
    private readonly DialogManager _dialogManager;
    private readonly SettingsService _settingsService;
    private readonly UpdateService _updateService;

    private readonly DisposableCollector _eventRoot = new();

    private readonly Timer _checkForUpdatesTimer;

    private bool _isInitialized;

    public LocalizationManager LocalizationManager { get; }

    public DashboardViewModel Dashboard { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TrayShowHideMenuItemHeader))]
    public partial bool IsWindowVisible { get; set; }

    public string TrayShowHideMenuItemHeader =>
        IsWindowVisible
            ? LocalizationManager.TrayHideMenuItem
            : LocalizationManager.TrayShowMenuItem;

    public MainViewModel(
        ViewModelManager viewModelManager,
        DialogManager dialogManager,
        SettingsService settingsService,
        LocalizationManager localizationManager,
        UpdateService updateService
    )
    {
        _viewModelManager = viewModelManager;
        _dialogManager = dialogManager;
        _settingsService = settingsService;
        _updateService = updateService;

        LocalizationManager = localizationManager;
        Dashboard = viewModelManager.CreateDashboardViewModel();

        _checkForUpdatesTimer = new Timer(
            TimeSpan.FromHours(3),
            async () =>
            {
                try
                {
                    var updateVersion = await updateService.CheckForUpdatesAsync();
                    if (updateVersion is not null)
                        await updateService.PrepareUpdateAsync(updateVersion);
                }
                catch
                {
                    // Failure to update shouldn't crash the application
                }
            }
        );

        _eventRoot.Add(
            localizationManager.WatchProperty(
                o => o.Language,
                () => OnPropertyChanged(nameof(TrayShowHideMenuItemHeader))
            )
        );
    }

    private async Task FinalizePendingUpdateAsync()
    {
        var updateVersion = _updateService.TryGetLastPreparedUpdate();
        if (updateVersion is null)
            return;

        var dialog = _viewModelManager.CreateMessageBoxViewModel(
            LocalizationManager.UpdateAvailableTitle,
            string.Format(LocalizationManager.UpdateAvailableMessage, Program.Name, updateVersion),
            LocalizationManager.InstallButton,
            LocalizationManager.CancelButton
        );

        // Bring the user's attention to this dialog, even if the app is hidden
        Application.Current?.ApplicationLifetime?.TryGetMainWindow()?.ShowActivateFocus();

        if (await _dialogManager.ShowDialogAsync(dialog) != true)
            return;

        _updateService.FinalizeUpdate(updateVersion);

        if (Application.Current?.ApplicationLifetime?.TryShutdown(2) != true)
            Environment.Exit(2);
    }

    private async Task ShowUkraineSupportMessageAsync()
    {
        if (!_settingsService.IsUkraineSupportMessageEnabled)
            return;

        var dialog = _viewModelManager.CreateMessageBoxViewModel(
            LocalizationManager.UkraineSupportTitle,
            LocalizationManager.UkraineSupportMessage,
            LocalizationManager.LearnMoreButton,
            LocalizationManager.CloseButton
        );

        // Disable this message in the future
        _settingsService.IsUkraineSupportMessageEnabled = false;
        _settingsService.Save();

        if (await _dialogManager.ShowDialogAsync(dialog) == true)
            Process.StartShellExecute("https://tyrrrz.me/ukraine?source=lightbulb");
    }

    private async Task ShowDevelopmentBuildMessageAsync()
    {
        if (!Program.IsDevelopmentBuild)
            return;

        // If debugging, the user is likely a developer
        if (Debugger.IsAttached)
            return;

        var dialog = _viewModelManager.CreateMessageBoxViewModel(
            LocalizationManager.UnstableBuildTitle,
            string.Format(LocalizationManager.UnstableBuildMessage, Program.Name),
            LocalizationManager.SeeReleasesButton,
            LocalizationManager.CloseButton
        );

        if (await _dialogManager.ShowDialogAsync(dialog) == true)
            Process.StartShellExecute(Program.ProjectReleasesUrl);
    }

    private async Task ShowGammaRangeMessageAsync()
    {
        if (_settingsService.IsExtendedGammaRangeUnlocked)
            return;

        var dialog = _viewModelManager.CreateMessageBoxViewModel(
            LocalizationManager.LimitedGammaRangeTitle,
            string.Format(LocalizationManager.LimitedGammaRangeMessage, Program.Name),
            LocalizationManager.FixButton,
            LocalizationManager.CloseButton
        );

        if (await _dialogManager.ShowDialogAsync(dialog) != true)
            return;

        _settingsService.IsExtendedGammaRangeUnlocked = true;
        _settingsService.Save();
    }

    private async Task ShowFirstTimeExperienceMessageAsync()
    {
        if (!_settingsService.IsFirstTimeExperienceEnabled)
            return;

        var dialog = _viewModelManager.CreateMessageBoxViewModel(
            LocalizationManager.WelcomeTitle,
            string.Format(LocalizationManager.WelcomeMessage, Program.Name),
            LocalizationManager.OkButton,
            LocalizationManager.CloseButton
        );

        // Disable this message in the future
        _settingsService.IsFirstTimeExperienceEnabled = false;
        _settingsService.IsAutoStartEnabled = true;
        _settingsService.Save();

        if (await _dialogManager.ShowDialogAsync(dialog) != true)
            return;

        var settingsDialog = _viewModelManager.CreateSettingsViewModel();
        settingsDialog.ActivateTab<LocationSettingsTabViewModel>();

        await _dialogManager.ShowDialogAsync(settingsDialog);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;

        await FinalizePendingUpdateAsync();

        await ShowUkraineSupportMessageAsync();
        await ShowDevelopmentBuildMessageAsync();
        await ShowGammaRangeMessageAsync();
        await ShowFirstTimeExperienceMessageAsync();

        _checkForUpdatesTimer.Start();
    }

    [RelayCommand]
    private async Task ShowSettingsAsync() =>
        await _dialogManager.ShowDialogAsync(_viewModelManager.CreateSettingsViewModel());

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

        await ShowSettingsAsync();
    }

    [RelayCommand]
    private void ToggleWindow() => App.Current?.ToggleMainWindow();

    [RelayCommand]
    private void Exit()
    {
        if (Application.Current?.ApplicationLifetime?.TryShutdown() != true)
            Environment.Exit(0);
    }

    [RelayCommand]
    private void ShowAbout() => Process.StartShellExecute(Program.ProjectUrl);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _checkForUpdatesTimer.Dispose();
            _eventRoot.Dispose();
        }

        base.Dispose(disposing);
    }
}
