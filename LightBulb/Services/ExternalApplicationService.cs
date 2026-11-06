using System;
using System.Collections.Generic;
using System.IO;
using LightBulb.Models;
using LightBulb.PlatformInterop;

namespace LightBulb.Services;

public class ExternalApplicationService
{
    // Applications that we don't want to show to the user
    private readonly HashSet<string> _ignoredApplicationNames = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        "explorer",
        // Windows shell experience host (hosts the task switcher, notification center, etc.)
        "ShellExperienceHost",
        // Windows start menu experience host
        "StartMenuExperienceHost",
    };

    private static string? TryGetExecutableFilePath(Window? window)
    {
        using var process = window?.TryGetProcess();
        var executableFilePath = process?.TryGetExecutableFilePath();

        return !string.IsNullOrWhiteSpace(executableFilePath) ? executableFilePath : null;
    }

    private bool IsIgnored(string executableFilePath)
    {
        var executableFileName = Path.GetFileNameWithoutExtension(executableFilePath);

        return string.IsNullOrWhiteSpace(executableFileName)
            || _ignoredApplicationNames.Contains(executableFileName);
    }

    public IEnumerable<ExternalApplication> GetAllRunningApplications()
    {
        foreach (var window in Window.GetAll())
        {
            using var _ = window;

            if (!window.IsVisible() || window.IsSystemWindow())
                continue;

            var executableFilePath = TryGetExecutableFilePath(window);
            if (executableFilePath is null || IsIgnored(executableFilePath))
                continue;

            yield return new ExternalApplication(executableFilePath);
        }
    }

    public ExternalApplication? TryGetForegroundApplication()
    {
        using var window = Window.TryGetForeground();

        var executableFilePath = TryGetExecutableFilePath(window);
        if (executableFilePath is null || IsIgnored(executableFilePath))
            return null;

        return new ExternalApplication(executableFilePath);
    }

    public bool IsForegroundApplicationFullScreen()
    {
        using var window = Window.TryGetForeground();
        if (window is null || !window.IsVisible() || window.IsSystemWindow())
            return false;

        var executableFilePath = TryGetExecutableFilePath(window);
        if (executableFilePath is not null && IsIgnored(executableFilePath))
            return false;

        return window.IsFullScreen();
    }
}
