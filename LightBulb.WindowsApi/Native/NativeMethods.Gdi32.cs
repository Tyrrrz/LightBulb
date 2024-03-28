using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Native;

internal static partial class NativeMethods
{
    private const string Gdi32 = "gdi32.dll";

    [DllImport(Gdi32, SetLastError = true)]
    public static extern nint CreateDC(
        string? lpszDriver,
        string? lpszDevice,
        string? lpszOutput,
        nint lpInitData
    );

    [DllImport(Gdi32, SetLastError = true)]
    public static extern bool DeleteDC(nint hdc);

    [DllImport(Gdi32, SetLastError = true)]
    public static extern bool GetDeviceGammaRamp(nint hdc, out GammaRamp lpRamp);

    [DllImport(Gdi32, SetLastError = true)]
    public static extern bool SetDeviceGammaRamp(nint hdc, ref GammaRamp lpRamp);
}
