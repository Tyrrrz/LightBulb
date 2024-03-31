using System;
using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct PowerBroadcastSetting(Guid PowerSettingId);
