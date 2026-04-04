using System.Collections.Generic;

namespace LightBulb.PlatformInterop;

/// <summary>
/// Returns the <see cref="IDisplayColorController"/> implementations available on Windows.
/// </summary>
public static class GammaController
{
    public static IReadOnlyList<IDisplayColorController> GetAvailable() =>
        [new WindowsGdiNativeDisplayGammaController()];
}
