using System.Runtime.InteropServices;
using LightBulb.WindowsApi.Types;

namespace LightBulb.WindowsApi;

internal static partial class NativeMethods
{
    private const string Gdi32 = "gdi32.dll";

    [DllImport(Gdi32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern nint CreateDC(
        string? lpszDriver,
        string? lpszDevice,
        string? lpszOutput,
        nint lpInitData
    );

    [DllImport(Gdi32, SetLastError = true)]
    public static extern bool DeleteDC(nint hDc);

    [DllImport(Gdi32, SetLastError = true)]
    public static extern bool GetDeviceGammaRamp(nint hDc, out GammaRamp lpRamp);

    [DllImport(Gdi32, SetLastError = true)]
    public static extern bool SetDeviceGammaRamp(nint hDc, ref GammaRamp lpRamp);
}
