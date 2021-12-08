using System;
using System.Diagnostics;
using LightBulb.WindowsApi.Native;
using LightBulb.WindowsApi.Utils.Extensions;

namespace LightBulb.WindowsApi;

public partial class PowerSettingNotification : IDisposable
{
    private readonly IDisposable _wndProcRegistration;

    public IntPtr Handle { get; }

    public Guid PowerSettingId { get; }

    public Action Callback { get; }

    public PowerSettingNotification(IntPtr handle, Guid powerSettingId, Action callback)
    {
        Handle = handle;
        PowerSettingId = powerSettingId;
        Callback = callback;

        _wndProcRegistration = WndProc.Listen(536, m =>
        {
            // Filter out other power events
            if (m.GetLParam<PowerBroadcastSetting>().PowerSettingId != powerSettingId)
                return;

            Callback();
        });
    }

    ~PowerSettingNotification() => Dispose();

    public void Dispose()
    {
        _wndProcRegistration.Dispose();

        if (!NativeMethods.UnregisterPowerSettingNotification(Handle))
            Debug.WriteLine("Could not dispose power setting event.");

        GC.SuppressFinalize(this);
    }
}

public partial class PowerSettingNotification
{
    public static Guid ConsoleDisplayStateId { get; } = Guid.Parse("6FE69556-704A-47A0-8F24-C28D936FDA47");

    public static Guid MonitorPowerOnId { get; } = Guid.Parse("02731015-4510-4526-99E6-E5A17EBD1AEA");

    public static Guid PowerSavingStatusId { get; } = Guid.Parse("E00958C0-C213-4ACE-AC77-FECCED2EEEA5");

    public static Guid SessionDisplayStatusId { get; } = Guid.Parse("2B84C20E-AD23-4ddf-93DB-05FFBD7EFCA5");

    public static Guid AwayModeId { get; } = Guid.Parse("98A7F580-01F7-48AA-9C0F-44352C29E5C0");

    public static PowerSettingNotification? TryRegister(Guid powerSettingId, Action callback)
    {
        var handle = NativeMethods.RegisterPowerSettingNotification(WndProc.Handle, ref powerSettingId, 0);
        return handle != IntPtr.Zero
            ? new PowerSettingNotification(handle, powerSettingId, callback)
            : null;
    }
}