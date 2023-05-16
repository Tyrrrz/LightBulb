using System;
using System.Linq;
using LightBulb.Core;
using LightBulb.Core.Utils.Extensions;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.Utils.Extensions;
using LightBulb.WindowsApi;
using Stylet;

namespace LightBulb.ViewModels.Components;

public class DashboardViewModel : PropertyChangedBase, IDisposable
{
    private readonly SettingsService _settingsService;
    private readonly GammaService _gammaService;
    private readonly HotKeyService _hotKeyService;
    private readonly ExternalApplicationService _externalApplicationService;

    private readonly Timer _updateInstantTimer;
    private readonly Timer _updateConfigurationTimer;
    private readonly Timer _updateIsPausedTimer;

    private IDisposable? _enableAfterDelayRegistration;

    public bool IsEnabled { get; set; } = true;

    public bool IsPaused { get; private set; }

    public bool IsCyclePreviewEnabled { get; set; }

    public bool IsActive => IsEnabled && !IsPaused || IsCyclePreviewEnabled;

    public DateTimeOffset Instant { get; private set; } = DateTimeOffset.Now;

    public SolarTimes SolarTimes =>
        !_settingsService.IsManualSunriseSunsetEnabled &&
        _settingsService.Location is { } location
            ? SolarTimes.Calculate(location, Instant)
            : new SolarTimes(_settingsService.ManualSunrise, _settingsService.ManualSunset);

    public TimeOnly SunriseStart => Cycle.GetSunriseStart(
        SolarTimes.Sunrise,
        _settingsService.ConfigurationTransitionDuration,
        _settingsService.ConfigurationTransitionOffset
    );

    public TimeOnly SunriseEnd => Cycle.GetSunriseEnd(
        SolarTimes.Sunrise,
        _settingsService.ConfigurationTransitionDuration,
        _settingsService.ConfigurationTransitionOffset
    );

    public TimeOnly SunsetStart => Cycle.GetSunsetStart(
        SolarTimes.Sunset,
        _settingsService.ConfigurationTransitionDuration,
        _settingsService.ConfigurationTransitionOffset
    );

    public TimeOnly SunsetEnd => Cycle.GetSunsetEnd(
        SolarTimes.Sunset,
        _settingsService.ConfigurationTransitionDuration,
        _settingsService.ConfigurationTransitionOffset
    );

    public double TemperatureOffset { get; set; }

    public double BrightnessOffset { get; set; }

    public ColorConfiguration TargetConfiguration => IsActive
        ? Cycle
            .InterpolateConfiguration(
                SolarTimes,
                _settingsService.DayConfiguration,
                _settingsService.NightConfiguration,
                _settingsService.ConfigurationTransitionDuration,
                _settingsService.ConfigurationTransitionOffset,
                Instant
            )
            .WithOffset(
                TemperatureOffset,
                BrightnessOffset
            )
            .Clamp(
                _settingsService.MinimumTemperature,
                _settingsService.MaximumTemperature,
                _settingsService.MinimumBrightness,
                _settingsService.MaximumBrightness
            )
        : _settingsService.IsDefaultToDayConfigurationEnabled
            ? _settingsService.DayConfiguration
            : ColorConfiguration.Default;

    public ColorConfiguration CurrentConfiguration { get; set; } = ColorConfiguration.Default;

    public ColorConfiguration AdjustedDayConfiguration => _settingsService.DayConfiguration.WithOffset(
        TemperatureOffset,
        BrightnessOffset
    );

    public ColorConfiguration AdjustedNightConfiguration => _settingsService.NightConfiguration.WithOffset(
        TemperatureOffset,
        BrightnessOffset
    );

    public CycleState CycleState => this switch
    {
        _ when CurrentConfiguration != TargetConfiguration => CycleState.Transition,
        _ when !IsEnabled => CycleState.Disabled,
        _ when IsPaused => CycleState.Paused,
        _ when CurrentConfiguration == AdjustedDayConfiguration => CycleState.Day,
        _ when CurrentConfiguration == AdjustedNightConfiguration => CycleState.Night,
        _ => CycleState.Transition
    };

    public DashboardViewModel(
        SettingsService settingsService,
        GammaService gammaService,
        HotKeyService hotKeyService,
        ExternalApplicationService externalApplicationService)
    {
        _settingsService = settingsService;
        _gammaService = gammaService;
        _hotKeyService = hotKeyService;
        _externalApplicationService = externalApplicationService;

        _updateConfigurationTimer = new Timer(
            TimeSpan.FromMilliseconds(50),
            UpdateConfiguration
        );

        _updateInstantTimer = new Timer(
            TimeSpan.FromMilliseconds(50),
            UpdateInstant
        );

        _updateIsPausedTimer = new Timer(
            TimeSpan.FromSeconds(1),
            UpdateIsPaused
        );

        // Cancel 'disable temporarily' when switching to enabled
        this.Bind(o => o.IsEnabled, (_, _) =>
        {
            if (IsEnabled)
                _enableAfterDelayRegistration?.Dispose();
        });

        // Handle settings changes
        _settingsService.SettingsSaved += (_, _) =>
        {
            Refresh();
            RegisterHotKeys();
        };
    }

    public void OnViewLoaded()
    {
        _updateInstantTimer.Start();
        _updateConfigurationTimer.Start();
        _updateIsPausedTimer.Start();

        RegisterHotKeys();
    }

    private void RegisterHotKeys()
    {
        _hotKeyService.UnregisterAllHotKeys();

        if (_settingsService.ToggleHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(_settingsService.ToggleHotKey, Toggle);
        }

        if (_settingsService.IncreaseTemperatureOffsetHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(_settingsService.IncreaseTemperatureOffsetHotKey, () =>
            {
                TemperatureOffset +=
                    Math.Min(100, _settingsService.MaximumTemperature - TargetConfiguration.Temperature);
            });
        }

        if (_settingsService.DecreaseTemperatureOffsetHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(_settingsService.DecreaseTemperatureOffsetHotKey, () =>
            {
                TemperatureOffset +=
                    Math.Max(-100, _settingsService.MinimumTemperature - TargetConfiguration.Temperature);
            });
        }

        if (_settingsService.IncreaseBrightnessOffsetHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(_settingsService.IncreaseBrightnessOffsetHotKey, () =>
            {
                BrightnessOffset +=
                    Math.Min(0.05, _settingsService.MaximumBrightness - TargetConfiguration.Brightness);
            });
        }

        if (_settingsService.DecreaseBrightnessOffsetHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(_settingsService.DecreaseBrightnessOffsetHotKey, () =>
            {
                BrightnessOffset +=
                    Math.Max(-0.05, _settingsService.MinimumBrightness - TargetConfiguration.Brightness);
            });
        }

        if (_settingsService.ResetConfigurationOffsetHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(
                _settingsService.ResetConfigurationOffsetHotKey,
                ResetConfigurationOffset
            );
        }
    }

    private void UpdateInstant()
    {
        // If in cycle preview mode - advance quickly until full cycle
        if (IsCyclePreviewEnabled)
        {
            // Cycle is supposed to end 1 full day past current real time
            var targetInstant = DateTimeOffset.Now + TimeSpan.FromDays(1);

            Instant = Instant.StepTo(targetInstant, TimeSpan.FromMinutes(5));
            if (Instant >= targetInstant)
                IsCyclePreviewEnabled = false;
        }
        // Otherwise - synchronize instant with system clock
        else
        {
            Instant = DateTimeOffset.Now;
        }
    }

    private void UpdateConfiguration()
    {
        var isSmooth = _settingsService.IsConfigurationSmoothingEnabled && !IsCyclePreviewEnabled;

        CurrentConfiguration = isSmooth
            ? CurrentConfiguration.StepTo(TargetConfiguration, 30, 0.008)
            : TargetConfiguration;

        _gammaService.SetGamma(CurrentConfiguration);
    }

    private void UpdateIsPaused()
    {
        bool IsPausedByFullScreen() =>
            _settingsService.IsPauseWhenFullScreenEnabled &&
            _externalApplicationService.IsForegroundApplicationFullScreen();

        bool IsPausedByWhitelistedApplication() =>
            _settingsService.IsApplicationWhitelistEnabled &&
            _settingsService.WhitelistedApplications is not null &&
            _settingsService.WhitelistedApplications.Contains(_externalApplicationService.TryGetForegroundApplication());

        IsPaused = IsPausedByFullScreen() || IsPausedByWhitelistedApplication();
    }

    public void Enable() => IsEnabled = true;

    public void Disable() => IsEnabled = false;

    public void DisableTemporarily(TimeSpan duration)
    {
        _enableAfterDelayRegistration?.Dispose();
        _enableAfterDelayRegistration = Timer.QueueDelayedAction(duration, Enable);
        IsEnabled = false;
    }

    public void DisableTemporarilyUntilSunrise()
    {
        // Use real time here instead of Instant, because that's what the user likely wants
        var now = DateTimeOffset.Now;
        var timeUntilSunrise = SolarTimes.Sunrise.NextAfter(now) - now;
        DisableTemporarily(timeUntilSunrise);
    }

    public void Toggle() => IsEnabled = !IsEnabled;

    public void EnableCyclePreview() => IsCyclePreviewEnabled = true;

    public void DisableCyclePreview() => IsCyclePreviewEnabled = false;

    public bool CanResetConfigurationOffset =>
        Math.Abs(TemperatureOffset) + Math.Abs(BrightnessOffset) >= 0.01;

    public void ResetConfigurationOffset()
    {
        TemperatureOffset = 0;
        BrightnessOffset = 0;
    }

    public void Dispose()
    {
        _updateInstantTimer.Dispose();
        _updateConfigurationTimer.Dispose();
        _updateIsPausedTimer.Dispose();

        _enableAfterDelayRegistration?.Dispose();
    }
}