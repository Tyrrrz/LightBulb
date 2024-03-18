using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;
using LightBulb.ViewModels.Framework;
using LightBulb.WindowsApi;

namespace LightBulb.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly ViewModelLocator _viewModelLocator;
    private readonly DialogManager _dialogManager;
    private readonly SettingsService _settingsService;

    private readonly Timer _checkForUpdatesTimer;

    public DashboardViewModel Dashboard { get; }

    public MainViewModel(
        ViewModelLocator viewModelLocator,
        DialogManager dialogManager,
        SettingsService settingsService,
        UpdateService updateService
    )
    {
        _viewModelLocator = viewModelLocator;
        _dialogManager = dialogManager;
        _settingsService = settingsService;

        _checkForUpdatesTimer = new Timer(
            TimeSpan.FromHours(3),
            async () => await updateService.CheckPrepareUpdateAsync()
        );

        Dashboard = viewModelLocator.GetDashboardViewModel();
    }

    private async Task ShowGammaRangePromptAsync()
    {
        if (_settingsService.IsExtendedGammaRangeUnlocked)
            return;

        var dialog = _viewModelLocator.GetMessageBoxViewModel(
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

        var dialog = _viewModelLocator.GetMessageBoxViewModel(
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
            var settingsDialog = _viewModelLocator.GetSettingsViewModel();
            settingsDialog.ActivateTabByType<LocationSettingsTabViewModel>();

            await _dialogManager.ShowDialogAsync(settingsDialog);
        }
    }

    private async Task ShowUkraineSupportMessageAsync()
    {
        if (!_settingsService.IsUkraineSupportMessageEnabled)
            return;

        var dialog = _viewModelLocator.GetMessageBoxViewModel(
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
        await _dialogManager.ShowDialogAsync(_viewModelLocator.GetSettingsViewModel());

    [RelayCommand]
    private void ShowAbout() => ProcessEx.StartShellExecute(App.ProjectUrl);

    public void Dispose() => _checkForUpdatesTimer.Dispose();
}
