using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace LightBulb.Utils.Extensions;

internal static class AvaloniaExtensions
{
    public static Window? TryGetMainWindow(this IApplicationLifetime applicationLifetime)
    {
        if (applicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            return lifetime.MainWindow;

        return null;
    }
    
    public static bool TryShutdown(this IApplicationLifetime applicationLifetime, int exitCode = 0)
    {
        if (applicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Shutdown(exitCode);
            return true;
        }

        return false;
    }
}