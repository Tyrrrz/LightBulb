using System;
using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Native;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct PowerBroadcastSetting(Guid PowerSettingId, uint DataLength, byte Data);