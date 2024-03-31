using System;
using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

internal partial class WndProcSponge(NativeWindowClass windowClass, NativeWindow window) : IDisposable
{
    public IDisposable Listen(Action<
    
    public void Dispose()
    {
        windowClass.Dispose();
        window.Dispose();
    }
}

internal partial class WndProcSponge
{
    private static readonly Lazy<WndProcSponge> DefaultLazy =
        new(() => TryCreate() ?? throw new InvalidOperationException("Failed to create WndProc sponge."));
    
    public static WndProcSponge Default => DefaultLazy.Value;
    
    public static WndProcSponge? TryCreate()
    {
        var windowClass = NativeWindowClass.TryCreate("LightBulb.WndProcSponge", (_, msg, wParam, lParam) =>
        {
            if (msg == 0x0010)
            {
                NativeMethods.PostQuitMessage(0);
                return 0;
            }
            
            return NativeMethods.DefWindowProc(_, msg, wParam, lParam);
        });

        if (windowClass is null)
            return null;
        
        var window = NativeWindow.TryCreate(windowClass, "LightBulb.WndProcSponge");
        if (window is null)
        {
            windowClass.Dispose();
            return null;
        }

        return new WndProcSponge(windowClass, window);
    }
}