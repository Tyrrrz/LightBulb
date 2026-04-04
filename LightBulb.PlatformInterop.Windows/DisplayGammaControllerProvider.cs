using System.Collections.Generic;

namespace LightBulb.PlatformInterop;

/// <summary>
/// Returns the <see cref="IDisplayGammaController"/> implementations available on Windows.
/// </summary>
public static class DisplayGammaControllerProvider
{
    public static IReadOnlyList<IDisplayGammaController> GetAvailable() =>
        [new WindowsNativeDisplayGammaController()];
}
