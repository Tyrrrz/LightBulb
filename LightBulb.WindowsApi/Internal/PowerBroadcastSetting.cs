using System;
using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Internal
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PowerBroadcastSetting
    {
        public Guid PowerSetting;
        public uint DataLength;
        public byte Data;
    }
}

