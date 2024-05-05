using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LightBulb.Framework;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;
using LightBulb.ViewModels;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;
using LightBulb.ViewModels.Dialogs;
using LightBulb.Views;
using Microsoft.Extensions.DependencyInjection;

namespace LightBulb;

public class App : Application, IDisposable
{
    private readonly DisposableCollector _eventRoot = new();

    private readonly ServiceProvider _services;
    private readonly MainViewModel _mainViewModel;

    public App()
    {
        var services = new ServiceCollection();

        // Framework
        services.AddSingleton<DialogManager>();
        services.AddSingleton<ViewManager>();
        services.AddSingleton<ViewModelManager>();

        // Services
        services.AddSingleton<ExternalApplicationService>();
        services.AddSingleton<GammaService>();
        services.AddSingleton<HotKeyService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<UpdateService>();

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

        _services = services.BuildServiceProvider(true);
        _mainViewModel = _services.GetRequiredService<ViewModelManager>().CreateMainViewModel();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Tray icon does not support binding so we use this hack to update its tooltip
        _eventRoot.Add(
            _mainViewModel.Dashboard.WatchProperties(
                [o => o.IsActive, o => o.CurrentConfiguration],
                () =>
                {
                    var status =
                        _mainViewModel.Dashboard.CurrentConfiguration.Temperature.ToString("F0")
                        + " / "
                        + _mainViewModel.Dashboard.CurrentConfiguration.Brightness.ToString("P0");

                    var tooltip =
                        "LightBulb"
                        + Environment.NewLine
                        + (_mainViewModel.Dashboard.IsActive ? status : "Disabled");

                    Dispatcher.UIThread.Invoke(() =>
                    {
                        if (TrayIcon.GetIcons(this)?.FirstOrDefault() is { } trayIcon)
                            trayIcon.ToolTipText = tooltip;
                    });
                }
            )
        );
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            desktopLifetime.MainWindow = new MainView { DataContext = _mainViewModel };

        base.OnFrameworkInitializationCompleted();
    }

    private void ShowMainWindow()
    {
        if (ApplicationLifetime?.TryGetMainWindow() is { } window)
        {
            window.Show();
            window.Activate();
            window.Focus();
        }
    }

    private void TrayIcon_OnClicked(object? sender, EventArgs args) => ShowMainWindow();

    private void ShowSettingsMenuItem_OnClick(object? sender, EventArgs args)
    {
        ShowMainWindow();
        _mainViewModel.ShowSettingsCommand.Execute(null);
    }

    private void ToggleMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.IsEnabled = !_mainViewModel.Dashboard.IsEnabled;

    private void DisableUntilSunriseMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableUntilSunriseCommand.Execute(null);

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

    private void ExitMenuItem_OnClick(object? sender, EventArgs args) =>
        ApplicationLifetime?.TryShutdown();

    public void Dispose()
    {
        _eventRoot.Dispose();
        _services.Dispose();
    }
}
