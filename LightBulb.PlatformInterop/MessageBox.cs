using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

public static class MessageBox
{
    public static bool ShowError(string title, string message) =>
        NativeMethods.MessageBox(0, message, title, 0x10) == 0;
}
