using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using LightBulb.Services;
using LightBulb.Utils.Extensions;
using LightBulb.ViewModels;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;
using LightBulb.ViewModels.Dialogs;
using LightBulb.ViewModels.Framework;
using LightBulb.Views;
using LightBulb.Views.Framework;
using Material.Styles.Themes;
using Microsoft.Extensions.DependencyInjection;

namespace LightBulb;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public ViewModelProvider ViewModelProvider => _services.GetRequiredService<ViewModelProvider>();

    // These view models are exposed here to set up bindings for the tray icon menu,
    // which must be defined in the application layout.
    public MainViewModel MainViewModel => ViewModelProvider.GetMainViewModel();
    public DashboardViewModel DashboardViewModel => ViewModelProvider.GetDashboardViewModel();

    public App()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ExternalApplicationService>();
        services.AddSingleton<GammaService>();
        services.AddSingleton<HotKeyService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<UpdateService>();

        services.AddSingleton<DialogManager>();
        services.AddSingleton<ViewModelProvider>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<MessageBoxViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<SettingsTabViewModelBase, AdvancedSettingsTabViewModel>();
        services.AddSingleton<SettingsTabViewModelBase, ApplicationWhitelistSettingsTabViewModel>();
        services.AddSingleton<SettingsTabViewModelBase, GeneralSettingsTabViewModel>();
        services.AddSingleton<SettingsTabViewModelBase, HotKeySettingsTabViewModel>();
        services.AddSingleton<SettingsTabViewModelBase, LocationSettingsTabViewModel>();

        services.AddSingleton<ViewLocator>();

        _services = services.BuildServiceProvider();
    }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainView
            {
                DataContext = ViewModelProvider.GetMainViewModel()
            };
        }

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

    private void ExitMenuItem_OnClick(object? sender, EventArgs args) =>
        ApplicationLifetime?.TryShutdown();
}

public partial class App
{
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    public static Version Version { get; } = Assembly.GetName().Version!;

    public static string VersionString { get; } = Version.ToString(3);

    public static string ExecutableDirPath { get; } = AppDomain.CurrentDomain.BaseDirectory;

    public static string ExecutableFilePath { get; } =
        Path.ChangeExtension(Assembly.Location, "exe");

    public static string ProjectUrl { get; } = "https://github.com/Tyrrrz/LightBulb";
}

public partial class App
{
    private static IReadOnlyList<string> CommandLineArgs { get; } =
        Environment.GetCommandLineArgs().Skip(1).ToArray();

    public static string StartHiddenArgument { get; } = "--start-hidden";

    public static bool IsHiddenOnLaunch { get; } =
        CommandLineArgs.Contains(StartHiddenArgument, StringComparer.OrdinalIgnoreCase);
}
