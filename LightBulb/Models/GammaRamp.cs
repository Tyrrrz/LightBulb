using System.Runtime.InteropServices;

namespace LightBulb.Models
{
    /// <summary>
    /// Gamma correction curve
    /// <remarks>WinAPI struct</remarks>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GammaRamp
    {
        /// <summary>
        /// Red channel output levels
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public readonly ushort[] Red;

        /// <summary>
        /// Green channel output levels
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public readonly ushort[] Green;

        /// <summary>
        /// Blue channel output levels
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public readonly ushort[] Blue;

        public GammaRamp(int size = 256)
        {
            Red = new ushort[size];
            Green = new ushort[size];
            Blue = new ushort[size];
        }
    }
}