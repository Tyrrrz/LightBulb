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
)
{
    /// <summary>
    /// Returns the <see cref="Data"/> value as a 32-bit integer, validating that the
    /// underlying payload size matches <see cref="sizeof(int)"/> (4 bytes).
    /// </summary>
    public int GetDataAsInt32()
    {
        if (DataLength != sizeof(int))
        {
            throw new InvalidOperationException(
                $"PowerBroadcastSetting.Data has length {DataLength} bytes; expected {sizeof(int)} bytes for a 32-bit integer value."
            );
        }

        return Data;
    }
}
