using System;
using System.Diagnostics;

namespace LightBulb.PlatformInterop;

public static class MessageBox
{
    public static void ShowError(string title, string message)
    {
        // On Linux, surface errors via stderr and trace so they are not silently swallowed
        var formatted = $"[{title}] {message}";
        Console.Error.WriteLine(formatted);
        Trace.TraceError(formatted);
    }
}
