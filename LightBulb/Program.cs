using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Avalonia;
using LightBulb.PlatformInterop;

namespace LightBulb;

public static class Program
{
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    public static string Name { get; } = Assembly.GetName().Name ?? "LightBulb";

    public static Version Version { get; } = Assembly.GetName().Version ?? new Version(0, 0, 0);

    public static string VersionString { get; } = Version.ToString(3);

    public static string ExecutableDirPath { get; } = AppContext.BaseDirectory;

    public static string ExecutableFilePath { get; } =
        Path.ChangeExtension(Assembly.Location, "exe");

    public static string ProjectUrl { get; } = "https://github.com/Tyrrrz/LightBulb";

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();

    [STAThread]
    public static int Main(string[] args)
    {
        // Ensure only one instance of the app is running at a time
        using var identityMutex = new Mutex(
            true,
            $"{Name}_Identity",
            out var isOnlyRunningInstance
        );

        if (!isOnlyRunningInstance)
            return 1;

        // Build and run the app
        var builder = BuildAvaloniaApp();

        try
        {
            return builder.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            MessageBox.ShowError("Fatal Error", ex.ToString());
            throw;
        }
        finally
        {
            // Clean up after application shutdown
            if (builder.Instance is IDisposable disposableApp)
                disposableApp.Dispose();
        }
    }
}
