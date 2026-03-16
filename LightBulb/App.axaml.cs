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

public partial class App : Application, IDisposable
{
    public static new App? Current => Application.Current as App;

    private readonly DisposableCollector _eventRoot = new();

    private readonly ServiceProvider _services;
    private readonly SettingsService _settingsService;
    private readonly LocalizationManager _localizationManager;
    private readonly MainViewModel _mainViewModel;

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
        _localizationManager = _services.GetRequiredService<LocalizationManager>();
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

        RegisterTrayIconEvents();
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
        {
            desktopLifetime.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs args)
            {
                if (sender is IControlledApplicationLifetime lifetime)
                    lifetime.Exit -= OnExit;

                Dispose();
            }

            // Although `App.Dispose()` is invoked from `Program.Main(...)`, on some platforms
            // it may be called too late in the shutdown lifecycle. Attach an exit
            // handler to ensure timely disposal as a safeguard.
            // https://github.com/Tyrrrz/YoutubeDownloader/issues/795
            desktopLifetime.Exit += OnExit;
        }

        base.OnFrameworkInitializationCompleted();

        // Load settings first so the saved theme is applied before showing the window.
        // Previously, settings were loaded after InitializeTheme(), which caused the saved
        // theme to not take effect on the first render (the WatchProperty callback triggered
        // an async Material.Avalonia theme update that raced with the window's initial paint).
        _settingsService.Load();

        // Set up custom theme colors based on the now-loaded settings
        InitializeTheme();

        // Build the tray icon and menu in code so we hold direct references to each item
        InitializeTrayIcon();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime dl)
        {
            if (!StartOptions.Current.IsInitiallyHidden)
            {
                // Show the main window on startup (after theme is configured)
                ShowMainWindow();
            }
            else
            {
                // When starting hidden, initialize the backend without showing the UI
                _mainViewModel.Dashboard.InitializeCommand.Execute(null);
            }
        }
    }

    internal Window? ShowMainWindow()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktopLifetime)
            return null;

        // Re-use the existing window if already open
        if (desktopLifetime.MainWindow is { } existingWindow)
        {
            existingWindow.ShowActivateFocus();
        }
        // Otherwise, create a new window (the previous one was closed to free resources)
        else
        {
            var window = new MainView { DataContext = _mainViewModel };
            window.Closed += (_, _) => desktopLifetime.MainWindow = null;
            desktopLifetime.MainWindow = window;
            window.ShowActivateFocus();
        }

        return desktopLifetime.MainWindow;
    }

    internal void ToggleMainWindow()
    {
        var existingWindow = ApplicationLifetime?.TryGetMainWindow();

        if (existingWindow is { IsVisible: true })
            existingWindow.Close();
        else
            ShowMainWindow();
    }

    private void Application_OnActualThemeVariantChanged(object? sender, EventArgs args) =>
        // Re-initialize the theme when the system theme changes
        InitializeTheme();

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        _eventRoot.Dispose();
        _services.Dispose();
    }
}
