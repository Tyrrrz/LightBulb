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
        UpdateService updateService)
    {
        _viewModelFactory = viewModelFactory;
        _dialogManager = dialogManager;
        _settingsService = settingsService;

        Dashboard = viewModelFactory.CreateDashboardViewModel();

        DisplayName = $"{App.Name} v{App.VersionString}";

        _checkForUpdatesTimer = new Timer(
            TimeSpan.FromHours(3),
            async () => await updateService.CheckPrepareUpdateAsync()
        );
    }

    private async Task ShowGammaRangePromptAsync()
    {
        if (_settingsService.IsExtendedGammaRangeUnlocked)
            return;

        var dialog = _viewModelFactory.CreateMessageBoxViewModel(
            "Limited gamma range", $@"
{App.Name} has detected that extended gamma range has not yet been unlocked on this computer.
This may cause some color configurations to not display properly.

Press OK to unlock gamma range. Administrator privileges may be required.".Trim(),
            "OK", "CANCEL"
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
            "Welcome!", $@"
Thank you for installing {App.Name}!
To get the most personalized experience, please set your preferred solar configuration.

Press OK to open settings.".Trim(),
            "OK", "CANCEL"
        );

        // Disable first time experience in the future
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

    private async Task ShowWarInUkraineMessageAsync()
    {
        var dialog = _viewModelFactory.CreateMessageBoxViewModel(
            "Ukraine is at war!", @"
My country, Ukraine, has been invaded by Russian military forces in an act of aggression that can only be described as genocide.
Be on the right side of history! Consider supporting Ukraine in its fight for freedom.

Press LEARN MORE to find ways that you can help.".Trim(),
            "LEARN MORE", "CLOSE"
        );

        if (await _dialogManager.ShowDialogAsync(dialog) == true)
        {
            ProcessEx.StartShellExecute("https://tyrrrz.me");
        }
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
        await ShowWarInUkraineMessageAsync();
    }

    public async void ShowSettings() =>
        await _dialogManager.ShowDialogAsync(_viewModelFactory.CreateSettingsViewModel());

    public void ShowAbout() => ProcessEx.StartShellExecute(App.GitHubProjectUrl);

    public void Exit() => Application.Current.Shutdown();

    public void Dispose()
    {
        _checkForUpdatesTimer.Dispose();
    }
}