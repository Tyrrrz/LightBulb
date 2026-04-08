using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBulb.Core;
using LightBulb.PlatformInterop;

namespace LightBulb.Services;

public partial class GammaService : IDisposable
{
    private readonly SettingsService _settingsService;
    private readonly IReadOnlyList<IDisplayColorController> _availableControllers;
    private readonly DisplayStateWatcher _displayStateWatcher;

    private bool _isUpdatingGamma;

    private DateTimeOffset _lastGammaInvalidationTimestamp = DateTimeOffset.MinValue;
    private ColorConfiguration? _lastConfiguration;
    private DateTimeOffset _lastUpdateTimestamp = DateTimeOffset.MinValue;

    public IReadOnlyList<IDisplayColorController> AvailableControllers => _availableControllers;

    public GammaService(SettingsService settingsService)
    {
        _settingsService = settingsService;
        _availableControllers = GammaController.GetAvailable();

        _displayStateWatcher = DisplayStateWatcher.Create(
            InvalidateGamma,
            InvalidateDisplayConfiguration
        );
    }

    private IDisplayColorController GetActiveController()
    {
        if (_availableControllers.Count == 0)
            throw new InvalidOperationException("No display color controllers are available.");

        var id = _settingsService.DisplayGammaControllerId;
        if (id is not null)
        {
            var match = _availableControllers.FirstOrDefault(c => c.Id == id);
            if (match is not null)
                return match;
        }

        return _availableControllers[0];
    }

    private bool IsGammaStale()
    {
        var instant = DateTimeOffset.Now;

        // Assume gamma continues to be stale for some time after it has been invalidated
        if ((instant - _lastGammaInvalidationTimestamp).Duration() <= TimeSpan.FromSeconds(0.3))
            return true;

        // If polling is enabled, assume gamma is stale after some time has passed since the last update
        if (
            _settingsService.IsGammaPollingEnabled
            && (instant - _lastUpdateTimestamp).Duration() > TimeSpan.FromSeconds(1)
        )
            return true;

        return false;
    }

    private bool IsSignificantChange(ColorConfiguration configuration)
    {
        // Nothing to compare to
        if (_lastConfiguration is not { } lastConfiguration)
            return true;

        return Math.Abs(configuration.Temperature - lastConfiguration.Temperature) > 15
            || Math.Abs(configuration.Brightness - lastConfiguration.Brightness) > 0.01;
    }

    public void InvalidateGamma()
    {
        // Don't invalidate gamma when we're in the process of changing it ourselves,
        // to avoid an infinite loop.
        if (_isUpdatingGamma)
            return;

        _lastGammaInvalidationTimestamp = DateTimeOffset.Now;
        Debug.WriteLine("Gamma invalidated.");
    }

    public void InvalidateDisplayConfiguration()
    {
        GetActiveController().Invalidate();
        Debug.WriteLine("Display configuration invalidated.");
        InvalidateGamma();
    }

    public async Task SetGammaAsync(ColorConfiguration configuration)
    {
        // Avoid unnecessary changes as updating too often will cause stuttering
        if (!IsGammaStale() && !IsSignificantChange(configuration))
            return;

        _isUpdatingGamma = true;

        await GetActiveController().SetGammaAsync(configuration);

        _isUpdatingGamma = false;

        _lastConfiguration = configuration;
        _lastUpdateTimestamp = DateTimeOffset.Now;
        Debug.WriteLine($"Updated gamma to {configuration}.");
    }

    public void Dispose()
    {
        foreach (var controller in _availableControllers)
        {
            controller
                .ResetGammaAsync()
                .AsTask()
                .ContinueWith(
                    t =>
                        Debug.WriteLine(
                            $"Failed to reset gamma on dispose for '{controller.Id}': {t.Exception}"
                        ),
                    TaskContinuationOptions.OnlyOnFaulted
                );
            controller.Dispose();
        }

        _displayStateWatcher.Dispose();
    }
}
