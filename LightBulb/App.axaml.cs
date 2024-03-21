using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using LightBulb.Framework;
using LightBulb.Services;
using LightBulb.Utils.Extensions;
using LightBulb.ViewModels;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;
using LightBulb.ViewModels.Dialogs;
using LightBulb.Views;
using Material.Styles.Themes;
using Microsoft.Extensions.DependencyInjection;

namespace LightBulb;

public class App : Application
{
    private readonly IServiceProvider _services;
    private readonly MainViewModel _mainViewModel;

    public App()
    {
        var services = new ServiceCollection();

        // Services
        services.AddSingleton<ExternalApplicationService>();
        services.AddSingleton<GammaService>();
        services.AddSingleton<HotKeyService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<UpdateService>();

        // View model framework
        services.AddSingleton<DialogManager>();
        services.AddSingleton<ViewModelProvider>();

        // View models
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<MessageBoxViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsTabViewModelBase, AdvancedSettingsTabViewModel>();
        services.AddTransient<SettingsTabViewModelBase, ApplicationWhitelistSettingsTabViewModel>();
        services.AddTransient<SettingsTabViewModelBase, GeneralSettingsTabViewModel>();
        services.AddTransient<SettingsTabViewModelBase, HotKeySettingsTabViewModel>();
        services.AddTransient<SettingsTabViewModelBase, LocationSettingsTabViewModel>();

        // View framework
        services.AddSingleton<ViewLocator>();

        _services = services.BuildServiceProvider();
        _mainViewModel = _services.GetRequiredService<MainViewModel>();
    }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainView { DataContext = _mainViewModel };

        base.OnFrameworkInitializationCompleted();

        // Set custom theme colors
        this.LocateMaterialTheme<MaterialThemeBase>().CurrentTheme = Theme.Create(
            Theme.Light,
            Color.Parse("#343838"),
            Color.Parse("#F9A825")
        );

        // Finalize pending updates (and restart) before launching the app
        _services.GetRequiredService<UpdateService>().FinalizePendingUpdates();

        // Load settings
        _services.GetRequiredService<SettingsService>().Load();
    }

    private void TrayIcon_OnClicked(object? sender, EventArgs args)
    {
        if (ApplicationLifetime?.TryGetMainWindow() is { } window)
        {
            window.Show();
            window.Activate();
            window.Focus();
        }
    }

    private void ShowSettingsMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.ShowSettingsCommand.Execute(null);

    private void AboutMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.ShowAboutCommand.Execute(null);

    private void ToggleMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.IsEnabled = !_mainViewModel.Dashboard.IsEnabled;

    private void DisableUntilSunriseMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableUntilSunriseCommand.Execute(null);

    private void ExitMenuItem_OnClick(object? sender, EventArgs args) =>
        ApplicationLifetime?.TryShutdown();

    private void DisableTemporarily1DayMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromDays(1));

    private void DisableTemporarily12HoursMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(12));

    private void DisableTemporarily6HoursMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(6));

    private void DisableTemporarily3HoursMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(3));

    private void DisableTemporarily1HourMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(1));

    private void DisableTemporarily30MinutesMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(30));

    private void DisableTemporarily5MinutesMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(5));

    private void DisableTemporarily1MinuteMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(1));
}
