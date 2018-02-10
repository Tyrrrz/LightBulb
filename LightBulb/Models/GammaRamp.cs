using System.Runtime.InteropServices;

// ReSharper disable ConvertToAutoPropertyWhenPossible (marshaling)

namespace LightBulb.Models
{
    /// <summary>
    /// Gamma correction curve
    /// <remarks>WinAPI struct</remarks>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GammaRamp
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        private readonly ushort[] _red;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        private readonly ushort[] _green;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        private readonly ushort[] _blue;

        /// <summary>
        /// Red channel output levels
        /// </summary>
        public ushort[] Red => _red;

        /// <summary>
        /// Green channel output levels
        /// </summary>
        public ushort[] Green => _green;

        /// <summary>
        /// Blue channel output levels
        /// </summary>
        public ushort[] Blue => _blue;

        public GammaRamp(int size = 256)
        {
            _red = new ushort[size];
            _green = new ushort[size];
            _blue = new ushort[size];
        }
    }
}