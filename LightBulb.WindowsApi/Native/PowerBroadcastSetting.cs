using System;
using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct PowerBroadcastSetting
    {
        public Guid PowerSettingId { get; }

        public uint DataLength { get; }

        public byte Data { get; }

        public PowerBroadcastSetting(Guid powerSettingId, uint dataLength, byte data)
        {
            PowerSettingId = powerSettingId;
            DataLength = dataLength;
            Data = data;
        }
    }
}