using System.Collections.Generic;

namespace LightBulb.PlatformInterop;

/// <summary>
/// Returns the <see cref="IDisplayColorController"/> implementations available on Linux,
/// ordered from most capable (native) to most integrated (DE-specific).
/// </summary>
public static class GammaController
{
    public static IReadOnlyList<IDisplayColorController> GetAvailable()
    {
        var controllers = new List<IDisplayColorController>();

        // The Xorg/xrandr controller works on any X11 or XWayland session
        controllers.Add(new LinuxXorgDisplayGammaController());

        // DE-integrated controllers keep system settings in sync with LightBulb
        if (LinuxGnomeDisplayGammaController.IsAvailable)
            controllers.Add(new LinuxGnomeDisplayGammaController());

        if (LinuxPlasmaDisplayGammaController.IsAvailable)
            controllers.Add(new LinuxPlasmaDisplayGammaController());

        return controllers;
    }
}
