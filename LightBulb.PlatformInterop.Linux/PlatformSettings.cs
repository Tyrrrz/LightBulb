namespace LightBulb.PlatformInterop;

/// <summary>
/// Linux implementation of <see cref="IPlatformSettings"/>.
/// Auto-start via XDG .desktop files is not yet implemented; extended gamma range
/// is not applicable on Linux.
/// </summary>
public class PlatformSettings : IPlatformSettings
{
    // Parameters accepted for API symmetry with the Windows implementation.
    // TODO: use appName and autoStartCommand to configure
    // auto-start via ~/.config/autostart/<appName>.desktop
    public PlatformSettings(string appName, string autoStartCommand) { }

    public bool IsAutoStartEnabled { get; set; }
    public bool IsExtendedGammaRangeUnlocked { get; set; }
}
