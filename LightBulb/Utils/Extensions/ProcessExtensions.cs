using System.Diagnostics;

namespace LightBulb.Utils.Extensions;

internal static class ProcessExtensions
{
    extension(Process)
    {
        public static void StartShellExecute(string path)
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo(path) { UseShellExecute = true };
            process.Start();
        }
    }
}
