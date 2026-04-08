using System;
using System.Threading.Tasks;
using LightBulb.Core;

namespace LightBulb.PlatformInterop;

/// <summary>
/// Abstraction for controlling display color (temperature and optionally brightness).
/// </summary>
public interface IDisplayColorController : IDisposable
{
    /// <summary>Stable identifier used for persistence in settings.</summary>
    string Id { get; }

    /// <summary>Whether this controller supports adjusting brightness (as opposed to temperature only).</summary>
    bool IsBrightnessSupported { get; }

    /// <summary>Apply the given color configuration to the display(s).</summary>
    ValueTask SetGammaAsync(ColorConfiguration configuration);

    /// <summary>Reset display gamma to the neutral/default state.</summary>
    ValueTask ResetGammaAsync();

    /// <summary>
    /// Called when the display configuration has changed (e.g. a monitor was added or removed).
    /// Implementations that hold device contexts or cached display handles should invalidate
    /// their internal state so they re-enumerate on the next <see cref="SetGammaAsync"/> call.
    /// </summary>
    void Invalidate();
}
