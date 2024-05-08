namespace LightBulb.Models;

/// <summary>
/// Describes the application's theme mode.
/// </summary>
public enum ThemeMode
{
    /// <summary>
    /// Use the light theme
    /// </summary>
    Light,

    /// <summary>
    /// Use the dark theme
    /// </summary>
    Dark,

    /// <summary>
    /// Use whichever theme is specified by system settings
    /// </summary>
    System
}
