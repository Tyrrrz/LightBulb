using System;
using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Types;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct PowerBroadcastSetting(Guid PowerSettingId);