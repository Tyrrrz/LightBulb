using System;
using System.Linq;
using LightBulb.Domain;
using LightBulb.Internal;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.WindowsApi;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public class CoreViewModel : PropertyChangedBase, IDisposable
    {
        private readonly SettingsService _settingsService;
        private readonly GammaService _gammaService;
        private readonly HotKeyService _hotKeyService;
        private readonly ExternalApplicationService _externalApplicationService;

        private readonly SystemEventManager _systemEvents = new SystemEventManager();

        private readonly AutoResetTimer _updateInstantTimer;
        private readonly AutoResetTimer _updateConfigurationTimer;
        private readonly AutoResetTimer _updateIsPausedTimer;
        private readonly AutoResetTimer _pollingTimer;
        private readonly ManualResetTimer _enableAfterDelayTimer;

        private bool _isStale;

        public bool IsEnabled { get; set; } = true;

        public bool IsPaused { get; private set; }

        public bool IsCyclePreviewEnabled { get; set; }

        public bool IsActive => IsEnabled && !IsPaused || IsCyclePreviewEnabled;

        public DateTimeOffset Instant { get; private set; } = DateTimeOffset.Now;

        public SolarTimes SolarTimes =>
            !_settingsService.IsManualSunriseSunsetEnabled && _settingsService.Location != null
                ? SolarTimes.Calculate(_settingsService.Location.Value, Instant)
                : new SolarTimes(_settingsService.ManualSunrise, _settingsService.ManualSunset);

        public TimeOfDay SunriseStart =>
            SolarTimes.Sunrise - _settingsService.ConfigurationTransitionDuration;

        public TimeOfDay SunsetEnd =>
            SolarTimes.Sunset + _settingsService.ConfigurationTransitionDuration;

        public ColorConfiguration TargetColorConfiguration => IsActive
            ? ColorConfiguration.Calculate(
                SolarTimes,
                _settingsService.DayConfiguration,
                _settingsService.NightConfiguration,
                _settingsService.ConfigurationTransitionDuration,
                Instant)
            : _settingsService.IsDefaultToDayConfigurationEnabled
                ? _settingsService.DayConfiguration
                : ColorConfiguration.Default;

        public ColorConfiguration CurrentColorConfiguration { get; set; } = ColorConfiguration.Default;

        public CycleState CycleState => 42 switch
        {
            _ when CurrentColorConfiguration != TargetColorConfiguration => CycleState.Transition,
            _ when !IsEnabled => CycleState.Disabled,
            _ when IsPaused => CycleState.Paused,
            _ when CurrentColorConfiguration == _settingsService.DayConfiguration => CycleState.Day,
            _ when CurrentColorConfiguration == _settingsService.NightConfiguration => CycleState.Night,
            _ => CycleState.Transition
        };

        public CoreViewModel(
            SettingsService settingsService,
            GammaService gammaService,
            HotKeyService hotKeyService,
            ExternalApplicationService externalApplicationService)
        {
            _settingsService = settingsService;
            _gammaService = gammaService;
            _hotKeyService = hotKeyService;
            _externalApplicationService = externalApplicationService;

            _updateConfigurationTimer = new AutoResetTimer(UpdateConfiguration);
            _updateInstantTimer = new AutoResetTimer(UpdateInstant);
            _updateIsPausedTimer = new AutoResetTimer(UpdateIsPaused);
            _pollingTimer = new AutoResetTimer(PollGamma);
            _enableAfterDelayTimer = new ManualResetTimer(Enable);

            // Cancel 'disable temporarily' when switched to enabled
            this.Bind(o => o.IsEnabled, (sender, args) =>
            {
                if (IsEnabled)
                    _enableAfterDelayTimer.Stop();
            });

            // Handle settings changes
            _settingsService.SettingsSaved += (sender, args) =>
            {
                _isStale = true;
                Refresh();
                RegisterHotKeys();
            };

            // Handle display settings changes
            _systemEvents.DisplaySettingsChanged += (sender, args) => _isStale = true;
            _systemEvents.DisplayStateChanged += (sender, args) => _isStale = true;
        }

        public void OnViewFullyLoaded()
        {
            _updateInstantTimer.Start(TimeSpan.FromMilliseconds(50));
            _updateConfigurationTimer.Start(TimeSpan.FromMilliseconds(50));
            _updateIsPausedTimer.Start(TimeSpan.FromSeconds(1));
            _pollingTimer.Start(TimeSpan.FromSeconds(1));

            RegisterHotKeys();
        }

        private void RegisterHotKeys()
        {
            _hotKeyService.UnregisterAllHotKeys();

            if (_settingsService.ToggleHotKey != HotKey.None)
                _hotKeyService.RegisterHotKey(_settingsService.ToggleHotKey, Toggle);
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
            bool IsUpdateNeeded()
            {
                // Gamma is stale
                if (_isStale)
                    return true;

                // No change
                if (CurrentColorConfiguration == TargetColorConfiguration)
                    return false;

                // One of the extreme states
                if (TargetColorConfiguration == _settingsService.NightConfiguration ||
                    TargetColorConfiguration == _settingsService.DayConfiguration)
                    return true;

                // Change is too small
                if (Math.Abs(TargetColorConfiguration.Temperature - CurrentColorConfiguration.Temperature) < 25 &&
                    Math.Abs(TargetColorConfiguration.Brightness - CurrentColorConfiguration.Brightness) < 0.01)
                    return false;

                return true;
            }

            // Avoid redundant updates
            if (!IsUpdateNeeded())
                return;

            var isSmooth = _settingsService.IsConfigurationSmoothingEnabled && !IsCyclePreviewEnabled;

            CurrentColorConfiguration = isSmooth
                ? CurrentColorConfiguration.StepTo(TargetColorConfiguration, 30, 0.008)
                : TargetColorConfiguration;

            _gammaService.SetGamma(CurrentColorConfiguration);
            _isStale = false;
        }

        private void UpdateIsPaused()
        {
            bool IsPausedByFullScreen() =>
                _settingsService.IsPauseWhenFullScreenEnabled && _externalApplicationService.IsForegroundApplicationFullScreen();

            bool IsPausedByWhitelistedApplication() =>
                _settingsService.IsApplicationWhitelistEnabled && _settingsService.WhitelistedApplications != null &&
                _settingsService.WhitelistedApplications.Contains(_externalApplicationService.GetForegroundApplication());

            IsPaused = IsPausedByFullScreen() || IsPausedByWhitelistedApplication();
        }

        private void PollGamma()
        {
            if (!_settingsService.IsGammaPollingEnabled)
                return;

            _isStale = true;
        }

        public void Enable() => IsEnabled = true;

        public void Disable() => IsEnabled = false;

        public void DisableTemporarily(TimeSpan duration)
        {
            _enableAfterDelayTimer.Start(duration);
            IsEnabled = false;
        }

        public void DisableTemporarilyUntilSunrise()
        {
            // Use real time here instead of Instant, because that's what the user likely wants
            var timeUntilSunrise = SolarTimes.Sunrise.Next() - DateTimeOffset.Now;
            DisableTemporarily(timeUntilSunrise);
        }

        public void Toggle() => IsEnabled = !IsEnabled;

        public void EnableCyclePreview() => IsCyclePreviewEnabled = true;

        public void DisableCyclePreview() => IsCyclePreviewEnabled = false;

        public void Dispose()
        {
            _systemEvents.Dispose();
            _updateInstantTimer.Dispose();
            _updateConfigurationTimer.Dispose();
            _updateIsPausedTimer.Dispose();
            _pollingTimer.Dispose();
            _enableAfterDelayTimer.Dispose();
        }
    }
}