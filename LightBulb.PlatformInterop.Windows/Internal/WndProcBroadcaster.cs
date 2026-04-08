using System;

namespace LightBulb.PlatformInterop.Internal;

internal class WndProcBroadcaster
{
    public event EventHandler<WndProcMessage>? MessageBroadcasted;

    public void BroadcastMessage(uint message, nint wParam, nint lParam) =>
        MessageBroadcasted?.Invoke(this, new WndProcMessage(message, wParam, lParam));
}
