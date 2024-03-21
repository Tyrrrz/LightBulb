using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Avalonia;

namespace LightBulb;

public static partial class Program
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();

    [STAThread]
    public static int Main(string[] args)
    {
        using var identityMutex = new Mutex(
            true,
            $"{Name}_Identity",
            out var isOnlyRunningInstance
        );

        if (!isOnlyRunningInstance)
            return 0;

        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
}

public partial class Program
{
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    public static string Name { get; } = Assembly.GetName().Name ?? "LightBulb";

    public static Version Version { get; } = Assembly.GetName().Version ?? new Version(0, 0, 0);

    public static string VersionString { get; } = Version.ToString(3);

    public static string ExecutableDirPath { get; } = AppDomain.CurrentDomain.BaseDirectory;

    public static string ExecutableFilePath { get; } =
        Path.ChangeExtension(Assembly.Location, "exe");

    public static string ProjectUrl { get; } = "https://github.com/Tyrrrz/LightBulb";
}

public partial class Program
{
    private static IReadOnlyList<string> CommandLineArgs { get; } =
        Environment.GetCommandLineArgs().Skip(1).ToArray();

    public static string StartHiddenArgument { get; } = "--start-hidden";

    public static bool IsHiddenOnLaunch { get; } =
        CommandLineArgs.Contains(StartHiddenArgument, StringComparer.OrdinalIgnoreCase);
}
