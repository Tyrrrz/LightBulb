using System.Runtime.InteropServices;

namespace LightBulb.Utils;

internal static class NativeMethods
{
    public static class Windows
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox(nint hWnd, string text, string caption, uint type);
    }
}