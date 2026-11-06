using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using LightBulb.Framework;
using LightBulb.Localization;
using LightBulb.Services;
using LightBulb.Utils.Extensions;
using LightBulb.ViewModels;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;
using LightBulb.ViewModels.Dialogs;
using Material.Styles.Themes;
using Microsoft.Extensions.DependencyInjection;
using PowerKit.Extensions;

namespace LightBulb;

public partial class App : Application, IDisposable
{
    private readonly ServiceProvider _services;
    private readonly SettingsService _settingsService;
    private readonly MainViewModel _mainViewModel;

    private readonly IDisposable _eventSubscription;
    private bool _isDisposed;

    public App()
    {
        var services = new ServiceCollection();

        // Framework
        services.AddSingleton<DialogManager>();
        services.AddSingleton<ViewManager>();
        services.AddSingleton<ViewModelManager>();

        // Services
        services.AddSingleton<ExternalApplicationService>();
        services.AddSingleton<LocalizationManager>();
        services.AddSingleton<GammaService>();
        services.AddSingleton<HotKeyService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<UpdateService>();

        // View models
        services.AddTransient<MainViewModel>();
        services.AddSingleton<DashboardViewModel>();
        services.AddTransient<MessageBoxViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsTabViewModelBase, AdvancedSettingsTabViewModel>();
        services.AddTransient<SettingsTabViewModelBase, ApplicationWhitelistSettingsTabViewModel>();
        services.AddTransient<SettingsTabViewModelBase, GeneralSettingsTabViewModel>();
        services.AddTransient<SettingsTabViewModelBase, HotKeySettingsTabViewModel>();
        services.AddTransient<SettingsTabViewModelBase, LocationSettingsTabViewModel>();

        _services = services.BuildServiceProvider(true);
        _settingsService = _services.GetRequiredService<SettingsService>();
        _mainViewModel = _services.GetRequiredService<ViewModelManager>().GetMainViewModel();

        // Re-initialize the theme when the user changes it
        _eventSubscription = _settingsService.WatchProperty(
            o => o.Theme,
            v =>
            {
                RequestedThemeVariant = v switch
                {
                    ThemeVariant.Light => Avalonia.Styling.ThemeVariant.Light,
                    ThemeVariant.Dark => Avalonia.Styling.ThemeVariant.Dark,
                    _ => Avalonia.Styling.ThemeVariant.Default,
                };

                InitializeTheme();
            }
        );
    }

    public override void Initialize()
    {
        base.Initialize();

        AvaloniaXamlLoader.Load(this);

        // TrayIcon bindings resolve through the application's DataContext
        DataContext = _mainViewModel;
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
        // Load settings
        _settingsService.Load();

        // Initialize the lifetime
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Although `App.Dispose()` is invoked from `Program.Main(...)`, on some platforms
            // it may be called too late in the shutdown lifecycle. Attach an exit
            // handler to ensure timely disposal as a safeguard.
            // https://github.com/Tyrrrz/YoutubeDownloader/issues/795
            desktop.Exit += (_, _) => Dispose();

            // Initialize the theme regardless of whether the main window is shown on
            // startup, otherwise it may not be applied correctly when starting hidden.
            // https://github.com/Tyrrrz/LightBulb/issues/432
            InitializeTheme();

            var viewManager = _services.GetRequiredService<ViewManager>();
            desktop.MainWindow = viewManager.TryBindWindow(_mainViewModel);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void Application_OnActualThemeVariantChanged(object? sender, EventArgs args) =>
        // Re-initialize the theme when the system theme changes
        InitializeTheme();

    private void TrayIcon_OnClicked(object? sender, EventArgs args) =>
        ApplicationLifetime?.TryGetMainWindow()?.Toggle();

    private void TrayToggleWindowMenuItem_OnClick(object? sender, EventArgs args) =>
        ApplicationLifetime?.TryGetMainWindow()?.Toggle();

    private void TrayShowSettingsMenuItem_OnClick(object? sender, EventArgs args)
    {
        ApplicationLifetime?.TryGetMainWindow()?.ShowActivateFocus();
        _mainViewModel.ShowSettingsCommand.Execute(null);
    }

    private void TrayExitMenuItem_OnClick(object? sender, EventArgs args) => Shutdown();

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        _eventSubscription.Dispose();
        _services.Dispose();
    }
}

public partial class App
{
    public static void Shutdown(int exitCode = 0)
    {
        if (Current?.ApplicationLifetime?.TryShutdown(exitCode) != true)
            Environment.Exit(exitCode);
    }
}
