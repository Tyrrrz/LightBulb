using System;
using System.Linq;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Core;
using LightBulb.Core.Utils.Extensions;
using LightBulb.Framework;
using LightBulb.Models;
using LightBulb.PlatformInterop;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;

namespace LightBulb.ViewModels.Components;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private readonly GammaService _gammaService;
    private readonly HotKeyService _hotKeyService;
    private readonly ExternalApplicationService _externalApplicationService;

    private readonly DisposableCollector _eventRoot = new();

    private readonly Timer _updateInstantTimer;
    private readonly Timer _updateConfigurationTimer;
    private readonly Timer _updateIsPausedTimer;

    private IDisposable? _enableAfterDelayRegistration;
    private ColorConfiguration? _configurationSmoothingSource;
    private ColorConfiguration? _configurationSmoothingTarget;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsActive))]
    public partial bool IsEnabled { get; set; } = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsActive))]
    public partial bool IsPaused { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsActive))]
    public partial bool IsCyclePreviewEnabled { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SolarTimes))]
    [NotifyPropertyChangedFor(nameof(SunriseStart))]
    [NotifyPropertyChangedFor(nameof(SunriseEnd))]
    [NotifyPropertyChangedFor(nameof(SunsetStart))]
    [NotifyPropertyChangedFor(nameof(SunsetEnd))]
    [NotifyPropertyChangedFor(nameof(TargetConfiguration))]
    [NotifyPropertyChangedFor(nameof(CycleState))]
    public partial DateTimeOffset Instant { get; set; } = DateTimeOffset.Now;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOffsetEnabled))]
    [NotifyPropertyChangedFor(nameof(TargetConfiguration))]
    [NotifyPropertyChangedFor(nameof(AdjustedDayConfiguration))]
    [NotifyPropertyChangedFor(nameof(AdjustedNightConfiguration))]
    [NotifyPropertyChangedFor(nameof(CycleState))]
    public partial double TemperatureOffset { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOffsetEnabled))]
    [NotifyPropertyChangedFor(nameof(TargetConfiguration))]
    [NotifyPropertyChangedFor(nameof(AdjustedDayConfiguration))]
    [NotifyPropertyChangedFor(nameof(AdjustedNightConfiguration))]
    [NotifyPropertyChangedFor(nameof(CycleState))]
    public partial double BrightnessOffset { get; set; }

    [ObservableProperty]
    public partial ColorConfiguration CurrentConfiguration { get; set; } =
        ColorConfiguration.Default;

    public DashboardViewModel(
        SettingsService settingsService,
        GammaService gammaService,
        HotKeyService hotKeyService,
        ExternalApplicationService externalApplicationService
    )
    {
        _settingsService = settingsService;
        _gammaService = gammaService;
        _hotKeyService = hotKeyService;
        _externalApplicationService = externalApplicationService;

        _eventRoot.Add(
            this.WatchProperty(
                o => o.IsEnabled,
                () =>
                {
                    if (IsEnabled)
                    {
                        // Cancel any activate 'disable temporarily' timers
                        _enableAfterDelayRegistration?.Dispose();

                        // Invalidate device contexts
                        _gammaService.InvalidateDeviceContexts();
                    }
                }
            )
        );

        _eventRoot.Add(
            // Re-register hotkeys when they get updated
            settingsService.WatchProperties(
                [
                    o => o.FocusWindowHotKey,
                    o => o.ToggleHotKey,
                    o => o.IncreaseTemperatureOffsetHotKey,
                    o => o.DecreaseTemperatureOffsetHotKey,
                    o => o.IncreaseBrightnessOffsetHotKey,
                    o => o.DecreaseBrightnessOffsetHotKey,
                    o => o.ResetConfigurationOffsetHotKey,
                ],
                RegisterHotKeys
            )
        );

        _updateConfigurationTimer = new Timer(TimeSpan.FromMilliseconds(50), UpdateConfiguration);
        _updateInstantTimer = new Timer(TimeSpan.FromMilliseconds(50), UpdateInstant);
        _updateIsPausedTimer = new Timer(TimeSpan.FromSeconds(1), UpdateIsPaused);
    }

    public bool IsActive => IsEnabled && !IsPaused || IsCyclePreviewEnabled;

    public SolarTimes SolarTimes =>
        _settingsService is { IsManualSunriseSunsetEnabled: false, Location: { } location }
            ? SolarTimes.Calculate(location, Instant)
            : new SolarTimes(_settingsService.ManualSunrise, _settingsService.ManualSunset);

    public TimeOnly SunriseStart =>
        Cycle.GetSunriseStart(
            SolarTimes.Sunrise,
            _settingsService.ConfigurationTransitionDuration,
            _settingsService.ConfigurationTransitionOffset
        );

    public TimeOnly SunriseEnd =>
        Cycle.GetSunriseEnd(
            SolarTimes.Sunrise,
            _settingsService.ConfigurationTransitionDuration,
            _settingsService.ConfigurationTransitionOffset
        );

    public TimeOnly SunsetStart =>
        Cycle.GetSunsetStart(
            SolarTimes.Sunset,
            _settingsService.ConfigurationTransitionDuration,
            _settingsService.ConfigurationTransitionOffset
        );

    public TimeOnly SunsetEnd =>
        Cycle.GetSunsetEnd(
            SolarTimes.Sunset,
            _settingsService.ConfigurationTransitionDuration,
            _settingsService.ConfigurationTransitionOffset
        );

    public bool IsOffsetEnabled => Math.Abs(TemperatureOffset) + Math.Abs(BrightnessOffset) >= 0.01;

    public ColorConfiguration TargetConfiguration =>
        IsActive
            ? Cycle
                .InterpolateConfiguration(
                    SolarTimes,
                    _settingsService.DayConfiguration,
                    _settingsService.NightConfiguration,
                    _settingsService.ConfigurationTransitionDuration,
                    _settingsService.ConfigurationTransitionOffset,
                    Instant
                )
                .WithOffset(TemperatureOffset, BrightnessOffset)
                .Clamp(
                    _settingsService.MinimumTemperature,
                    _settingsService.MaximumTemperature,
                    _settingsService.MinimumBrightness,
                    _settingsService.MaximumBrightness
                )
        : _settingsService.IsDefaultToDayConfigurationEnabled ? _settingsService.DayConfiguration
        : ColorConfiguration.Default;

    public ColorConfiguration AdjustedDayConfiguration =>
        _settingsService.DayConfiguration.WithOffset(TemperatureOffset, BrightnessOffset);

    public ColorConfiguration AdjustedNightConfiguration =>
        _settingsService.NightConfiguration.WithOffset(TemperatureOffset, BrightnessOffset);

    public CycleState CycleState =>
        this switch
        {
            _ when CurrentConfiguration != TargetConfiguration => CycleState.Transition,
            _ when !IsEnabled => CycleState.Disabled,
            _ when IsPaused => CycleState.Paused,
            _ when CurrentConfiguration == AdjustedDayConfiguration => CycleState.Day,
            _ when CurrentConfiguration == AdjustedNightConfiguration => CycleState.Night,
            _ => CycleState.Transition,
        };

    private void RegisterHotKeys()
    {
        _hotKeyService.UnregisterAllHotKeys();

        if (_settingsService.FocusWindowHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(
                _settingsService.FocusWindowHotKey,
                () => Application.Current?.TryFocusMainWindow()
            );
        }

        if (_settingsService.ToggleHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(
                _settingsService.ToggleHotKey,
                () => IsEnabled = !IsEnabled
            );
        }

        if (_settingsService.IncreaseTemperatureOffsetHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(
                _settingsService.IncreaseTemperatureOffsetHotKey,
                () =>
                {
                    TemperatureOffset += Math.Min(
                        100,
                        _settingsService.MaximumTemperature - TargetConfiguration.Temperature
                    );
                }
            );
        }

        if (_settingsService.DecreaseTemperatureOffsetHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(
                _settingsService.DecreaseTemperatureOffsetHotKey,
                () =>
                {
                    TemperatureOffset += Math.Max(
                        -100,
                        _settingsService.MinimumTemperature - TargetConfiguration.Temperature
                    );
                }
            );
        }

        if (_settingsService.IncreaseBrightnessOffsetHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(
                _settingsService.IncreaseBrightnessOffsetHotKey,
                () =>
                {
                    BrightnessOffset += Math.Min(
                        0.05,
                        _settingsService.MaximumBrightness - TargetConfiguration.Brightness
                    );
                }
            );
        }

        if (_settingsService.DecreaseBrightnessOffsetHotKey != HotKey.None)
        {
            _hotKeyService.RegisterHotKey(
                _settingsService.DecreaseBrightnessOffsetHotKey,
                () =>
                {
                    BrightnessOffset += Math.Max(
                        -0.05,
                        _settingsService.MinimumBrightness - TargetConfiguration.Brightness
                    );
                }
            );
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
        // If in cycle preview mode, advance quickly until the full cycle has been reached
        if (IsCyclePreviewEnabled)
        {
            // Cycle is supposed to end 1 full day past the current real time
            var targetInstant = DateTimeOffset.Now + TimeSpan.FromDays(1);

            Instant = Instant.StepTo(targetInstant, TimeSpan.FromMinutes(5));
            if (Instant >= targetInstant)
                IsCyclePreviewEnabled = false;
        }
        // Otherwise, synchronize the instant with the system clock
        else
        {
            Instant = DateTimeOffset.Now;
        }
    }

    private void UpdateConfiguration()
    {
        var isSmooth =
            !IsCyclePreviewEnabled
            && CurrentConfiguration != TargetConfiguration
            && _settingsService.IsConfigurationSmoothingEnabled
            && _settingsService.ConfigurationSmoothingMaxDuration.TotalSeconds >= 0.1;

        if (isSmooth)
        {
            // Check if the target configuration has changed since the last transition started
            if (
                _configurationSmoothingTarget != TargetConfiguration
                || _configurationSmoothingSource is null
            )
            {
                _configurationSmoothingSource = CurrentConfiguration;
                _configurationSmoothingTarget = TargetConfiguration;
            }

            var brightnessDelta = Math.Abs(
                _configurationSmoothingTarget.Value.Brightness
                    - _configurationSmoothingSource.Value.Brightness
            );

            var brightnessStep = Math.Max(
                brightnessDelta
                    / _settingsService.ConfigurationSmoothingMaxDuration.TotalSeconds
                    * _updateConfigurationTimer.Interval.TotalSeconds,
                0.08
            );

            var temperatureDelta = Math.Abs(
                _configurationSmoothingTarget.Value.Temperature
                    - _configurationSmoothingSource.Value.Temperature
            );

            var temperatureStep = Math.Max(
                temperatureDelta
                    / _settingsService.ConfigurationSmoothingMaxDuration.TotalSeconds
                    * _updateConfigurationTimer.Interval.TotalSeconds,
                30
            );

            CurrentConfiguration = CurrentConfiguration.StepTo(
                TargetConfiguration,
                temperatureStep,
                brightnessStep
            );
        }
        else
        {
            CurrentConfiguration = TargetConfiguration;
            _configurationSmoothingSource = null;
            _configurationSmoothingTarget = null;
        }

        _gammaService.SetGamma(CurrentConfiguration);
    }

    private void UpdateIsPaused()
    {
        bool IsPausedByFullScreen() =>
            _settingsService.IsPauseWhenFullScreenEnabled
            && _externalApplicationService.IsForegroundApplicationFullScreen();

        bool IsPausedByWhitelistedApplication() =>
            _settingsService.IsApplicationWhitelistEnabled
            && _settingsService.WhitelistedApplications is not null
            && _settingsService.WhitelistedApplications.Contains(
                _externalApplicationService.TryGetForegroundApplication()
            );

        IsPaused = IsPausedByFullScreen() || IsPausedByWhitelistedApplication();
    }

    [RelayCommand]
    private void Initialize()
    {
        _updateInstantTimer.Start();
        _updateConfigurationTimer.Start();
        _updateIsPausedTimer.Start();

        // Hack: feign property changes to refresh the tray icon
        OnAllPropertiesChanged();
    }

    [RelayCommand]
    private void DisableTemporarily(TimeSpan duration)
    {
        _enableAfterDelayRegistration?.Dispose();
        _enableAfterDelayRegistration = Timer.QueueDelayedAction(duration, () => IsEnabled = true);
        IsEnabled = false;
    }

    [RelayCommand]
    private void DisableUntilSunrise()
    {
        var now = DateTimeOffset.Now;
        var timeUntilSunrise = SolarTimes.Sunrise.NextAfter(now) - now;
        DisableTemporarily(timeUntilSunrise);
    }

    [RelayCommand]
    private void ResetConfigurationOffset()
    {
        TemperatureOffset = 0;
        BrightnessOffset = 0;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _eventRoot.Dispose();

            _updateInstantTimer.Dispose();
            _updateConfigurationTimer.Dispose();
            _updateIsPausedTimer.Dispose();

            _enableAfterDelayRegistration?.Dispose();
        }

        base.Dispose(disposing);
    }
}
