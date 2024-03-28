namespace LightBulb.WindowsApi;

public static class MessageBox
{
    public static bool ShowError(string title, string message) =>
        NativeMethods.MessageBox(0, message, title, 0x10) == 0;
}
