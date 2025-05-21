using System.Diagnostics;
using System.Runtime.InteropServices;
using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

public static class MessageBox
{
    public static void ShowError(string title, string message)
    {
        if (NativeMethods.MessageBox(0, message, title, 0x10) != 0)
        {
            Debug.WriteLine(
                "Failed to show message box. " + $"Error {Marshal.GetLastWin32Error()}."
            );
        }
    }
}
