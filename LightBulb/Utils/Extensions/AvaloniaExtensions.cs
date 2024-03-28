using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace LightBulb.Utils.Extensions;

internal static class AvaloniaExtensions
{
    public static Window? TryGetMainWindow(this IApplicationLifetime lifetime)
    {
        if (lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            return desktopLifetime.MainWindow;

        return null;
    }

    public static void Shutdown(this IApplicationLifetime lifetime, int exitCode = 0)
    {
        if (lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.TryShutdown(exitCode);
            return;
        }

        if (lifetime is IControlledApplicationLifetime controlledLifetime)
        {
            controlledLifetime.Shutdown(exitCode);
            return;
        }

        Environment.Exit(exitCode);
    }
}
