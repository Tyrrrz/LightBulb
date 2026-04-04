namespace LightBulb.PlatformInterop;

/// <summary>
/// Platform-specific settings that are stored outside the main JSON settings file
/// (e.g. in the OS registry on Windows, or via OS-specific mechanisms on other platforms).
/// </summary>
public interface IPlatformSettings
{
    bool IsAutoStartEnabled { get; set; }
    bool IsExtendedGammaRangeUnlocked { get; set; }
}
