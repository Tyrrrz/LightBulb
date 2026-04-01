using System;

namespace LightBulb.PlatformInterop;

public partial class DeviceContext : IDisposable
{
    private DeviceContext() { }

    public void SetGamma(double redMultiplier, double greenMultiplier, double blueMultiplier) { }

    public void ResetGamma() => SetGamma(1, 1, 1);

    public void Dispose() { }
}

public partial class DeviceContext
{
    public static DeviceContext? TryCreate(string deviceName) => null;
}
