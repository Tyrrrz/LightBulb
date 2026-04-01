namespace LightBulb.PlatformInterop;

public static class MessageBox
{
    public static void ShowError(string title, string message)
    {
        // Not implemented on Linux; errors should be surfaced through other means
    }
}
