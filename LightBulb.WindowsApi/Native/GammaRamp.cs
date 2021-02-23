using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GammaRamp
    {
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Red { get; set; }

        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Green { get; set; }

        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Blue { get; set; }
    }
}