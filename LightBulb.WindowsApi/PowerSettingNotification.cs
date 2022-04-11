using System;
using System.Diagnostics;
using LightBulb.WindowsApi.Native;
using LightBulb.WindowsApi.Utils.Extensions;

namespace LightBulb.WindowsApi;

public partial class PowerSettingNotification : IDisposable
{
    private readonly IntPtr _handle;

    private readonly IDisposable _wndProcRegistration;

    private PowerSettingNotification(IntPtr handle, Guid powerSettingId, Action callback)
    {
        _handle = handle;

        _wndProcRegistration = WndProc.Listen(WndProc.Ids.PowerSettingMessage, m =>
        {
            // Filter out other power events
            if (m.GetLParam<PowerBroadcastSetting>().PowerSettingId != powerSettingId)
                return;

            callback();
        });
    }

    ~PowerSettingNotification() => Dispose();

    public void Dispose()
    {
        _wndProcRegistration.Dispose();

        if (!NativeMethods.UnregisterPowerSettingNotification(_handle))
            Debug.WriteLine($"Failed to dispose power setting notification (handle: {_handle}).");

        GC.SuppressFinalize(this);
    }
}

public partial class PowerSettingNotification
{
    public static PowerSettingNotification? TryRegister(Guid powerSettingId, Action callback)
    {
        var handle = NativeMethods.RegisterPowerSettingNotification(WndProc.Handle, powerSettingId, 0);

        if (handle == IntPtr.Zero)
        {
            Debug.WriteLine($"Failed to register power setting notification (ID: {powerSettingId}).");
            return null;
        }

        return new PowerSettingNotification(handle, powerSettingId, callback);
    }
}

public partial class PowerSettingNotification
{
    public static class Ids
    {
        public static Guid ConsoleDisplayStateChanged { get; } = Guid.Parse("6FE69556-704A-47A0-8F24-C28D936FDA47");
        public static Guid MonitorPowerStateChanged { get; } = Guid.Parse("02731015-4510-4526-99E6-E5A17EBD1AEA");
        public static Guid PowerSavingStatusChanged { get; } = Guid.Parse("E00958C0-C213-4ACE-AC77-FECCED2EEEA5");
        public static Guid SessionDisplayStatusChanged { get; } = Guid.Parse("2B84C20E-AD23-4ddf-93DB-05FFBD7EFCA5");
        public static Guid AwayModeChanged { get; } = Guid.Parse("98A7F580-01F7-48AA-9C0F-44352C29E5C0");
    }
}