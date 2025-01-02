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
    };

    public IEnumerable<ExternalApplication> GetAllRunningApplications()
    {
        foreach (var window in Window.GetAll())
        {
            using var _ = window;

            if (!window.IsVisible() || window.IsSystemWindow())
                continue;

            using var process = window.TryGetProcess();

            var executableFilePath = process?.TryGetExecutableFilePath();
            var executableFileName = Path.GetFileNameWithoutExtension(executableFilePath);

            if (
                string.IsNullOrWhiteSpace(executableFilePath)
                || string.IsNullOrWhiteSpace(executableFileName)
            )
                continue;

            if (_ignoredApplicationNames.Contains(executableFileName))
                continue;

            yield return new ExternalApplication(executableFilePath);
        }
    }

    public ExternalApplication? TryGetForegroundApplication()
    {
        using var window = Window.TryGetForeground();
        using var process = window?.TryGetProcess();

        var executableFilePath = process?.TryGetExecutableFilePath();

        return !string.IsNullOrWhiteSpace(executableFilePath)
            ? new ExternalApplication(executableFilePath)
            : null;
    }

    public bool IsForegroundApplicationFullScreen()
    {
        using var window = Window.TryGetForeground();

        return window is not null
            && window.IsVisible()
            && !window.IsSystemWindow()
            && window.IsFullScreen();
    }
}
