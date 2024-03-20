using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Framework;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;
using LightBulb.WindowsApi;

namespace LightBulb.ViewModels;

public partial class MainViewModel : ViewModelBase, IDisposable
{
    private readonly ViewModelProvider _viewModelProvider;
    private readonly DialogManager _dialogManager;
    private readonly SettingsService _settingsService;

    private readonly Timer _checkForUpdatesTimer;

    public DashboardViewModel Dashboard { get; }

    public MainViewModel(
        ViewModelProvider viewModelProvider,
        DialogManager dialogManager,
        SettingsService settingsService,
        UpdateService updateService
    )
    {
        _viewModelProvider = viewModelProvider;
        _dialogManager = dialogManager;
        _settingsService = settingsService;

        _checkForUpdatesTimer = new Timer(
            TimeSpan.FromHours(3),
            async () => await updateService.CheckPrepareUpdateAsync()
        );

        Dashboard = viewModelProvider.GetDashboardViewModel();
    }

    private async Task ShowGammaRangePromptAsync()
    {
        if (_settingsService.IsExtendedGammaRangeUnlocked)
            return;

        var dialog = _viewModelProvider.GetMessageBoxViewModel(
            "Limited gamma range",
            """
            LightBulb has detected that extended gamma range controls are not enabled on this system.
            This may cause some color configurations to not work correctly.

            Press FIX to unlock the gamma range. Administrator privileges may be required.
            """,
            "FIX",
            "CLOSE"
        );

        if (await _dialogManager.ShowDialogAsync(dialog) == true)
        {
            _settingsService.IsExtendedGammaRangeUnlocked = true;
            _settingsService.Save();
        }
    }

    private async Task ShowFirstTimeExperienceMessageAsync()
    {
        if (!_settingsService.IsFirstTimeExperienceEnabled)
            return;

        var dialog = _viewModelProvider.GetMessageBoxViewModel(
            "Welcome!",
            """
            Thank you for installing LightBulb!
            To get the most personalized experience, please set your preferred solar configuration.

            Press OK to open settings.
            """,
            "OK",
            "CLOSE"
        );

        // Disable this message in the future
        _settingsService.IsFirstTimeExperienceEnabled = false;
        _settingsService.IsAutoStartEnabled = true;
        _settingsService.Save();

        if (await _dialogManager.ShowDialogAsync(dialog) == true)
        {
            var settingsDialog = _viewModelProvider.GetSettingsViewModel();
            settingsDialog.ActivateTab<LocationSettingsTabViewModel>();

            await _dialogManager.ShowDialogAsync(settingsDialog);
        }
    }

    private async Task ShowUkraineSupportMessageAsync()
    {
        if (!_settingsService.IsUkraineSupportMessageEnabled)
            return;

        var dialog = _viewModelProvider.GetMessageBoxViewModel(
            "Thank you for supporting Ukraine!",
            """
            As Russia wages a genocidal war against my country, I'm grateful to everyone who continues to stand with Ukraine in our fight for freedom.

            Click LEARN MORE to find ways that you can help.
            """,
            "LEARN MORE",
            "CLOSE"
        );

        // Disable this message in the future
        _settingsService.IsUkraineSupportMessageEnabled = false;
        _settingsService.Save();

        if (await _dialogManager.ShowDialogAsync(dialog) == true)
            ProcessEx.StartShellExecute("https://tyrrrz.me/ukraine?source=lightbulb");
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        await ShowGammaRangePromptAsync();
        await ShowFirstTimeExperienceMessageAsync();
        await ShowUkraineSupportMessageAsync();

        _checkForUpdatesTimer.Start();
    }

    [RelayCommand]
    private async Task ShowSettingsAsync() =>
        await _dialogManager.ShowDialogAsync(_viewModelProvider.GetSettingsViewModel());

    [RelayCommand]
    private void ShowAbout() => ProcessEx.StartShellExecute(App.ProjectUrl);

    public void Dispose() => _checkForUpdatesTimer.Dispose();
}
