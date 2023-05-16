using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LightBulb;

public partial class App
{
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    public static string Name { get; } = Assembly.GetName().Name!;

    public static Version Version { get; } = Assembly.GetName().Version!;

    public static string VersionString { get; } = Version.ToString(3);

    public static string ExecutableDirPath { get; } = AppDomain.CurrentDomain.BaseDirectory;

    public static string ExecutableFilePath { get; } = Path.ChangeExtension(Assembly.Location, "exe");

    public static string GitHubProjectUrl { get; } = "https://github.com/Tyrrrz/LightBulb";
}

public partial class App
{
    private static IReadOnlyList<string> CommandLineArgs { get; } = Environment
        .GetCommandLineArgs()
        .Skip(1)
        .ToArray();

    public static string HiddenOnLaunchArgument { get; } = "--start-hidden";

    public static bool IsHiddenOnLaunch { get; } = CommandLineArgs.Contains(
        HiddenOnLaunchArgument,
        StringComparer.OrdinalIgnoreCase
    );
}