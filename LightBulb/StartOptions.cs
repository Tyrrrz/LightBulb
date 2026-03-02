using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightBulb.PlatformInterop;
using LightBulb.Utils.Extensions;

namespace LightBulb;

public partial class StartOptions
{
    public bool IsInitiallyHidden { get; init; }

    public string SettingsPath { get; init; } = default!;
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
                        ? Path.Combine(path, "Settings.json")
                        : path
                    : GetDefaultSettingsPath(),
        };

    private static string GetDefaultSettingsPath()
    {
        var isInstalled = File.Exists(Path.Combine(Program.ExecutableDirPath, ".installed"));

        // Prefer storing settings in appdata when installed or when the current directory is write-protected
        if (isInstalled || !Directory.CheckWriteAccess(Program.ExecutableDirPath))
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Program.Name,
                "Settings.json"
            );
        }

        // Otherwise, store them in the current directory
        return Path.Combine(Program.ExecutableDirPath, "Settings.json");
    }
}
