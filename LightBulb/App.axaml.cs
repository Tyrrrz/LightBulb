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
using Material.Styles.Themes;

namespace LightBulb;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();

        base.OnFrameworkInitializationCompleted();

        // Set custom theme colors
        this.LocateMaterialTheme<MaterialThemeBase>().CurrentTheme = Theme.Create(
            Theme.Light,
            Color.Parse("#343838"),
            Color.Parse("#F9A825")
        );
    }

    public override void RegisterServices()
    {
        base.RegisterServices();
        
        // Finalize pending updates (and restart) before launching the app
        GetInstance<UpdateService>()
            .FinalizePendingUpdates();

        // Load settings (this has to come before any view is loaded because bindings are not updated)
        GetInstance<SettingsService>()
            .Load();
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
    
    private void ExitMenuItem_OnClick(object? sender, EventArgs args) => ApplicationLifetime?.TryShutdown();
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
