using System.Diagnostics;
using System.Runtime.InteropServices;
using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

public partial class NativeWindowClass(string name) : NativeResource<string>(name)
{
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!NativeMethods.UnregisterClass(Handle, 0)) 
                Debug.WriteLine($"Failed to unregister window class '{Handle}'.");
        }
    }
}

public partial class NativeWindowClass
{
    public static NativeWindowClass? TryCreate(string name, WndProc wndProc)
    {
        var wndClass = new WndClassEx
        {
            Size = (uint) Marshal.SizeOf<WndClassEx>(),
            Style = 0,
            WndProc = wndProc,
            ClassExtra = 0,
            WindowExtra = 0,
            Instance = 0,
            Icon = 0,
            Cursor = 0,
            Background = 0,
            MenuName = null,
            ClassName = name,
            IconSm = 0
        };
        
        if (NativeMethods.RegisterClassEx(ref wndClass) == 0)
        {
            Debug.WriteLine($"Failed to register window class '{name}'.");
            return null;
        }
        
        return new NativeWindowClass(name);
    }
}