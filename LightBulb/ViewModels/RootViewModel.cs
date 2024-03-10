using System;
using System.Threading.Tasks;
using System.Windows;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;
using LightBulb.ViewModels.Dialogs;
using LightBulb.ViewModels.Framework;
using LightBulb.WindowsApi;
using Stylet;

namespace LightBulb.ViewModels;

public class RootViewModel : Screen, IDisposable
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly DialogManager _dialogManager;
    private readonly SettingsService _settingsService;

    private readonly Timer _checkForUpdatesTimer;

    public DashboardViewModel Dashboard { get; }

    public RootViewModel(
        IViewModelFactory viewModelFactory,
        DialogManager dialogManager,
        SettingsService settingsService,
        UpdateService updateService
    )
    {
        _viewModelFactory = viewModelFactory;
        _dialogManager = dialogManager;
        _settingsService = settingsService;

        _checkForUpdatesTimer = new Timer(
            TimeSpan.FromHours(3),
            async () => await updateService.CheckPrepareUpdateAsync()
        );

        Dashboard = viewModelFactory.CreateDashboardViewModel();

        DisplayName = $"LightBulb v{App.VersionString}";
    }

    private async Task ShowGammaRangePromptAsync()
    {
        if (_settingsService.IsExtendedGammaRangeUnlocked)
            return;

        var dialog = _viewModelFactory.CreateMessageBoxViewModel(
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

        var dialog = _viewModelFactory.CreateMessageBoxViewModel(
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
            var settingsDialog = _viewModelFactory.CreateSettingsViewModel();
            settingsDialog.ActivateTabByType<LocationSettingsTabViewModel>();

            await _dialogManager.ShowDialogAsync(settingsDialog);
        }
    }

    private async Task ShowUkraineSupportMessageAsync()
    {
        if (!_settingsService.IsUkraineSupportMessageEnabled)
            return;

        var dialog = _viewModelFactory.CreateMessageBoxViewModel(
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

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        _checkForUpdatesTimer.Start();
    }

    // This is a custom event that fires when the dialog host is loaded
    public async void OnViewFullyLoaded()
    {
        await ShowGammaRangePromptAsync();
        await ShowFirstTimeExperienceMessageAsync();
        await ShowUkraineSupportMessageAsync();
    }

    public async void ShowSettings() =>
        await _dialogManager.ShowDialogAsync(_viewModelFactory.CreateSettingsViewModel());

    public void ShowAbout() => ProcessEx.StartShellExecute(App.ProjectUrl);

    public void Exit() => Application.Current.Shutdown();

    public void Dispose() => _checkForUpdatesTimer.Dispose();
}
