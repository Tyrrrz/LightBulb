using System;
using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct PowerBroadcastSetting(
    Guid PowerSettingId,
    // DataLength specifies the size of the Data field in bytes.
    // For display-related notifications, this is always 4 (a single DWORD).
    int DataLength,
    int Data
);
