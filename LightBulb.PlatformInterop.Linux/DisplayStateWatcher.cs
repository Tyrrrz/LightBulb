using System;

namespace LightBulb.PlatformInterop;

public partial class DisplayStateWatcher : IDisposable
{
    public DisplayStateWatcher(Action onGammaInvalidated, Action onDeviceContextInvalidated) { }

    public void Dispose() { }
}

public partial class DisplayStateWatcher
{
    public static DisplayStateWatcher Create(
        Action onGammaInvalidated,
        Action onDeviceContextInvalidated
    ) => new(onGammaInvalidated, onDeviceContextInvalidated);
}
