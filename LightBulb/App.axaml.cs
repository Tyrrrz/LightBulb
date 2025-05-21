using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
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
using Material.Styles.Themes;
using Microsoft.Extensions.DependencyInjection;

namespace LightBulb;

public class App : Application, IDisposable
{
    private readonly DisposableCollector _eventRoot = new();

    private readonly ServiceProvider _services;
    private readonly SettingsService _settingsService;
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
        _settingsService = _services.GetRequiredService<SettingsService>();
        _mainViewModel = _services.GetRequiredService<ViewModelManager>().CreateMainViewModel();

        // Re-initialize the theme when the user changes it
        _eventRoot.Add(
            _settingsService.WatchProperty(
                o => o.Theme,
                () =>
                {
                    RequestedThemeVariant = _settingsService.Theme switch
                    {
                        ThemeVariant.Light => Avalonia.Styling.ThemeVariant.Light,
                        ThemeVariant.Dark => Avalonia.Styling.ThemeVariant.Dark,
                        _ => Avalonia.Styling.ThemeVariant.Default,
                    };

                    InitializeTheme();
                }
            )
        );

        // Tray icon does not support binding so we use this hack to synchronize its tooltip
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

                    try
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            if (TrayIcon.GetIcons(this)?.FirstOrDefault() is { } trayIcon)
                                trayIcon.ToolTipText = tooltip;
                        });
                    }
                    // Ignore exceptions when the application is shutting down
                    catch (OperationCanceledException) { }
                }
            )
        );
    }

    public override void Initialize()
    {
        base.Initialize();

        AvaloniaXamlLoader.Load(this);
    }

    private void InitializeTheme()
    {
        var actualTheme = RequestedThemeVariant?.Key switch
        {
            "Light" => PlatformThemeVariant.Light,
            "Dark" => PlatformThemeVariant.Dark,
            _ => PlatformSettings?.GetColorValues().ThemeVariant ?? PlatformThemeVariant.Light,
        };

        this.LocateMaterialTheme<MaterialThemeBase>().CurrentTheme =
            actualTheme == PlatformThemeVariant.Light
                ? Theme.Create(Theme.Light, Color.Parse("#343838"), Color.Parse("#F9A825"))
                : Theme.Create(Theme.Dark, Color.Parse("#E8E8E8"), Color.Parse("#F9A825"));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            desktopLifetime.MainWindow = new MainView { DataContext = _mainViewModel };

        base.OnFrameworkInitializationCompleted();

        // Set up custom theme colors
        InitializeTheme();

        // Load settings
        _settingsService.Load();
    }

    private void Application_OnActualThemeVariantChanged(object? sender, EventArgs args) =>
        // Re-initialize the theme when the system theme changes
        InitializeTheme();

    private void TrayIcon_OnClicked(object? sender, EventArgs args) =>
        ApplicationLifetime?.TryGetMainWindow()?.Toggle();

    private void ShowSettingsMenuItem_OnClick(object? sender, EventArgs args)
    {
        ApplicationLifetime?.TryGetMainWindow()?.ShowActivateFocus();
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

    private void DisableTemporarily15MinutesMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(15));

    private void DisableTemporarily5MinutesMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(5));

    private void DisableTemporarily1MinuteMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(1));

    private void ExitMenuItem_OnClick(object? sender, EventArgs args)
    {
        if (ApplicationLifetime?.TryShutdown() != true)
            Environment.Exit(0);
    }

    public void Dispose()
    {
        _eventRoot.Dispose();
        _services.Dispose();
    }
}
