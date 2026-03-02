using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightBulb.PlatformInterop;
using LightBulb.Utils.Extensions;

namespace LightBulb;

public partial class StartOptions
{
    public required bool IsInitiallyHidden { get; init; }

    public required string SettingsPath { get; init; }
}

public partial class StartOptions
{
    public static string IsInitiallyHiddenArgument { get; } = "--start-hidden";

    public static StartOptions Current { get; } =
        Parse(Environment.GetCommandLineArgs().Skip(1).ToArray());

    public static StartOptions Parse(IReadOnlyList<string> commandLineArgs) =>
        new()
        {
            IsInitiallyHidden = commandLineArgs.Contains(
                IsInitiallyHiddenArgument,
                StringComparer.OrdinalIgnoreCase
            ),
            SettingsPath =
                Environment.GetEnvironmentVariable("LIGHTBULB_SETTINGS_PATH") is { } path
                && !string.IsNullOrWhiteSpace(path)
                    ? Path.EndsInDirectorySeparator(path) || Directory.Exists(path)
                        // Provided environment override, it's a directory path
                        ? Path.Combine(path, "Settings.json")
                        // Provided environment override, it's a file path
                        : path
                    : File.Exists(Path.Combine(Program.ExecutableDirPath, ".installed"))
                    || !Directory.CheckWriteAccess(Program.ExecutableDirPath)
                        // Cannot write to the program directory
                        ? Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            Program.Name,
                            "Settings.json"
                        )
                        // Can write to the program directory
                        : Path.Combine(Program.ExecutableDirPath, "Settings.json"),
        };
}
