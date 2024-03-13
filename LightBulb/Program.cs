using System;
using System.Threading;
using Avalonia;

namespace LightBulb;

public static class Program
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
    
    [STAThread]
    public static int Main(string[] args)
    {
        using var identityMutex = new Mutex(true, "LightBulb_Identity", out var isOnlyRunningInstance);
        if (!isOnlyRunningInstance)
            return 0;
        
        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
}