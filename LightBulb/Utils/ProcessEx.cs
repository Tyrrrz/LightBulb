using System.Diagnostics;

namespace LightBulb.Utils;

internal static class ProcessEx
{
    public static void StartShellExecute(string path)
    {
        var startInfo = new ProcessStartInfo(path)
        {
            UseShellExecute = true
        };

        using (Process.Start(startInfo))
        { }
    }
}