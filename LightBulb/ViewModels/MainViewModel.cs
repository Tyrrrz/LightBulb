using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Framework;
using LightBulb.PlatformInterop;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;

namespace LightBulb.ViewModels;

public partial class MainViewModel(
    ViewModelManager viewModelManager,
    DialogManager dialogManager,
    SettingsService settingsService,
    UpdateService updateService
) : ViewModelBase
{
    private readonly Timer _checkForUpdatesTimer =
        new(TimeSpan.FromHours(3), async () => await updateService.CheckPrepareUpdateAsync());

    public DashboardViewModel Dashboard { get; } = viewModelManager.CreateDashboardViewModel();

    private async Task ShowGammaRangePromptAsync()
    {
        if (settingsService.IsExtendedGammaRangeUnlocked)
            return;

        var dialog = viewModelManager.CreateMessageBoxViewModel(
            "Limited gamma range",
            $"""
            {Program.Name} has detected that extended gamma range controls are not enabled on this system.
            This may cause some color configurations to not work correctly.

            Press FIX to unlock the gamma range. Administrator privileges may be required.
            """,
            "FIX",
            "CLOSE"
        );

        if (await dialogManager.ShowDialogAsync(dialog) == true)
        {
            settingsService.IsExtendedGammaRangeUnlocked = true;
            settingsService.Save();
        }
    }

    private async Task ShowFirstTimeExperienceMessageAsync()
    {
        if (!settingsService.IsFirstTimeExperienceEnabled)
            return;

        var dialog = viewModelManager.CreateMessageBoxViewModel(
            "Welcome!",
            $"""
            Thank you for installing {Program.Name}!
            To get the most personalized experience, please set your preferred solar configuration.

            Press OK to open settings.
            """,
            "OK",
            "CLOSE"
        );

        // Disable this message in the future
        settingsService.IsFirstTimeExperienceEnabled = false;
        settingsService.IsAutoStartEnabled = true;
        settingsService.Save();

        if (await dialogManager.ShowDialogAsync(dialog) == true)
        {
            var settingsDialog = viewModelManager.CreateSettingsViewModel();
            settingsDialog.ActivateTab<LocationSettingsTabViewModel>();

            await dialogManager.ShowDialogAsync(settingsDialog);
        }
    }

    private async Task ShowUkraineSupportMessageAsync()
    {
        if (!settingsService.IsUkraineSupportMessageEnabled)
            return;

        var dialog = viewModelManager.CreateMessageBoxViewModel(
            "Thank you for supporting Ukraine!",
            """
            As Russia wages a genocidal war against my country, I'm grateful to everyone who continues to stand with Ukraine in our fight for freedom.

            Click LEARN MORE to find ways that you can help.
            """,
            "LEARN MORE",
            "CLOSE"
        );

        // Disable this message in the future
        settingsService.IsUkraineSupportMessageEnabled = false;
        settingsService.Save();

        if (await dialogManager.ShowDialogAsync(dialog) == true)
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
        await dialogManager.ShowDialogAsync(viewModelManager.CreateSettingsViewModel());

    [RelayCommand]
    private void ShowAbout() => ProcessEx.StartShellExecute(Program.ProjectUrl);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _checkForUpdatesTimer.Dispose();
        }

        base.Dispose(disposing);
    }
}
