using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Framework;
using LightBulb.PlatformInterop;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;
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
        new(
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

    public DashboardViewModel Dashboard { get; } = viewModelManager.CreateDashboardViewModel();

    private async Task FinalizePendingUpdateAsync()
    {
        var updateVersion = updateService.TryGetLastPreparedUpdate();
        if (updateVersion is null)
            return;

        var dialog = viewModelManager.CreateMessageBoxViewModel(
            "Update available",
            $"""
            Update to {Program.Name} v{updateVersion} has been downloaded.
            Do you want to install it now?
            """,
            "INSTALL",
            "CANCEL"
        );

        Application.Current?.TryFocusMainWindow();

        if (await dialogManager.ShowDialogAsync(dialog) != true)
            return;

        updateService.FinalizeUpdate(updateVersion);

        if (Application.Current?.ApplicationLifetime?.TryShutdown(2) != true)
            Environment.Exit(2);
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

        if (await dialogManager.ShowDialogAsync(dialog) != true)
            return;

        ProcessEx.StartShellExecute("https://tyrrrz.me/ukraine?source=lightbulb");
    }

    private async Task ShowDevelopmentBuildMessageAsync()
    {
        if (!Program.IsDevelopmentBuild)
            return;

        // If debugging, the user is likely a developer
        if (Debugger.IsAttached)
            return;

        var dialog = viewModelManager.CreateMessageBoxViewModel(
            "Unstable build warning",
            """
            You're using a development build of the application. These builds are not thoroughly tested and may contain bugs.

            Auto-updates are disabled for development builds. If you want to switch to a stable release, please download it manually.
            """,
            "SEE RELEASES",
            "CLOSE"
        );

        if (await dialogManager.ShowDialogAsync(dialog) == true)
            ProcessEx.StartShellExecute(Program.ProjectReleasesUrl);
    }

    private async Task ShowGammaRangeMessageAsync()
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

        if (await dialogManager.ShowDialogAsync(dialog) != true)
            return;

        settingsService.IsExtendedGammaRangeUnlocked = true;
        settingsService.Save();
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

        if (await dialogManager.ShowDialogAsync(dialog) != true)
            return;

        var settingsDialog = viewModelManager.CreateSettingsViewModel();
        settingsDialog.ActivateTab<LocationSettingsTabViewModel>();

        await dialogManager.ShowDialogAsync(settingsDialog);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        await FinalizePendingUpdateAsync();

        await ShowUkraineSupportMessageAsync();
        await ShowDevelopmentBuildMessageAsync();
        await ShowGammaRangeMessageAsync();
        await ShowFirstTimeExperienceMessageAsync();

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
            _checkForUpdatesTimer.Dispose();

        base.Dispose(disposing);
    }
}
