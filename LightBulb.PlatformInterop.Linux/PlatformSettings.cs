namespace LightBulb.PlatformInterop;

/// <summary>
/// Linux implementation of <see cref="IPlatformSettings"/>.
/// Auto-start via XDG .desktop files is not yet implemented; extended gamma range
/// is not applicable on Linux.
/// </summary>
public class PlatformSettings : IPlatformSettings
{
    // Stored for future use when implementing auto-start via ~/.config/autostart/
    private readonly string _appName;
    private readonly string _autoStartCommand;

    public PlatformSettings(string appName, string autoStartCommand)
    {
        _appName = appName;
        _autoStartCommand = autoStartCommand;
    }

    // TODO: implement auto-start via ~/.config/autostart/<_appName>.desktop using _autoStartCommand
    public bool IsAutoStartEnabled { get; set; }
    public bool IsExtendedGammaRangeUnlocked { get; set; }
}
