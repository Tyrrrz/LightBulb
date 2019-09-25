using System;
using System.Diagnostics;
using System.Reflection;
using LightBulb.Messages;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.Timers;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Framework;
using Stylet;

namespace LightBulb.ViewModels
{
    public class RootViewModel : Screen, IHandle<ToggleIsEnabledMessage>, IDisposable
    {
        private readonly SettingsService _settingsService;
        private readonly UpdateService _updateService;
        private readonly CalculationService _calculationService;
        private readonly LocationService _locationService;
        private readonly SystemService _systemService;

        private readonly AutoResetTimer _updateTimer;
        private readonly AutoResetTimer _gammaPollingTimer;
        private readonly AutoResetTimer _settingsAutoSaveTimer;
        private readonly AutoResetTimer _internetSyncTimer;
        private readonly AutoResetTimer _checkForUpdatesTimer;
        private readonly ManualResetTimer _enableAfterDelayTimer;

        public string ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public bool IsUpdateAvailable { get; private set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsPaused { get; private set; }

        public bool IsCyclePreviewEnabled { get; set; }

        public DateTimeOffset Instant { get; private set; } = DateTimeOffset.Now;

        public ColorTemperature TargetColorTemperature =>
            IsEnabled && !IsPaused || IsCyclePreviewEnabled
                ? _calculationService.CalculateColorTemperature(Instant)
                : ColorTemperature.Default;

        public ColorTemperature CurrentColorTemperature { get; private set; } = ColorTemperature.Default;

        public double CyclePosition => Instant.TimeOfDay.TotalDays;

        public CycleState CycleState
        {
            get
            {
                // If target temperature has not been reached - return transition
                if (CurrentColorTemperature != TargetColorTemperature)
                    return CycleState.Transition;

                // If at max temperature and enabled and not paused - return day
                if (CurrentColorTemperature == _settingsService.MaxTemperature && IsEnabled && !IsPaused)
                    return CycleState.Day;

                // If at min temperature and enabled and not paused - return night
                if (CurrentColorTemperature == _settingsService.MinTemperature && IsEnabled && !IsPaused)
                    return CycleState.Night;

                // Otherwise - return disabled
                return CycleState.Disabled;
            }
        }

        public string StatusText
        {
            get
            {
                // If in cycle preview - return current temperature and instant
                if (IsCyclePreviewEnabled)
                    return $"Temp: {CurrentColorTemperature}   Time: {Instant:t}";

                // If in transition or not disabled or paused - return current temperature
                if (CurrentColorTemperature != TargetColorTemperature || IsEnabled && !IsPaused)
                    return $"Temp: {CurrentColorTemperature}";

                // Otherwise - return "paused" or "disabled"
                return IsPaused ? "Paused" : "Disabled";
            }
        }

        public GeneralSettingsViewModel GeneralSettings { get; }

        public LocationSettingsViewModel LocationSettings { get; }

        public AdvancedSettingsViewModel AdvancedSettings { get; }

        public int SettingsIndex { get; private set; }

        public RootViewModel(IEventAggregator eventAggregator, IViewModelFactory viewModelFactory,
            SettingsService settingsService, UpdateService updateService,
            CalculationService calculationService, LocationService locationService,
            SystemService systemService)
        {
            _settingsService = settingsService;
            _updateService = updateService;
            _calculationService = calculationService;
            _locationService = locationService;
            _systemService = systemService;

            // Handle messages
            eventAggregator.Subscribe(this);

            // Initialize view models
            GeneralSettings = viewModelFactory.CreateGeneralSettingsViewModel();
            LocationSettings = viewModelFactory.CreateLocationSettingsViewModel();
            AdvancedSettings = viewModelFactory.CreateAdvancedSettingsViewModel();

            // When IsEnabled switches to 'true' - cancel 'disable temporarily'
            this.Bind(o => o.IsEnabled, (sender, args) =>
            {
                if (IsEnabled)
                    _enableAfterDelayTimer.Stop();
            });

            // Initialize timers
            _updateTimer = new AutoResetTimer(() =>
            {
                UpdateIsPaused();
                UpdateInstant();
                UpdateGamma();
            });

            _gammaPollingTimer = new AutoResetTimer(() =>
            {
                if (_settingsService.IsGammaPollingEnabled && IsEnabled && CurrentColorTemperature == TargetColorTemperature)
                    _systemService.SetGamma(CurrentColorTemperature);
            });

            _settingsAutoSaveTimer = new AutoResetTimer(() => _settingsService.SaveIfNeeded());

            _internetSyncTimer = new AutoResetTimer(async () =>
            {
                if (!_settingsService.IsInternetSyncEnabled)
                    return;

                // TODO: rework later
                var location = await _locationService.GetLocationAsync();

                if (_settingsService.IsInternetSyncEnabled)
                {
                    _settingsService.Location = location;

                    var instant = DateTimeOffset.Now;
                    _settingsService.SunriseTime = _calculationService.CalculateSunrise(location, instant).TimeOfDay;
                    _settingsService.SunsetTime = _calculationService.CalculateSunset(location, instant).TimeOfDay;
                }
            });

            _checkForUpdatesTimer = new AutoResetTimer(async () =>
            {
                IsUpdateAvailable = await _updateService.CheckForUpdatesAsync();
            });

            _enableAfterDelayTimer = new ManualResetTimer(Enable);
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            // Load settings
            _settingsService.Load();

            // Start timers
            _updateTimer.Start(TimeSpan.FromMilliseconds(17)); // 60hz
            _gammaPollingTimer.Start(TimeSpan.FromSeconds(1));
            _settingsAutoSaveTimer.Start(TimeSpan.FromSeconds(5));
            _internetSyncTimer.Start(TimeSpan.FromHours(3));
            _checkForUpdatesTimer.Start(TimeSpan.FromHours(3));
        }

        private void UpdateIsPaused()
        {
            IsPaused = _settingsService.IsPauseWhenFullScreenEnabled && _systemService.IsForegroundWindowFullScreen();
        }

        private void UpdateInstant()
        {
            // If in cycle preview mode - advance quickly until reached full cycle
            if (IsCyclePreviewEnabled)
            {
                // Cycle is supposed to end 1 full day past current real time
                var targetInstant = DateTimeOffset.Now + TimeSpan.FromDays(1);

                // Calculate difference
                var diff = targetInstant - Instant;

                // Calculate delta
                var delta = TimeSpan.FromMinutes(3);
                if (delta > diff)
                    delta = diff;

                // Set new instant
                Instant += delta;

                // If target instant reached - disable cycle preview
                if (Instant >= targetInstant)
                    IsCyclePreviewEnabled = false;
            }
            // Otherwise - simply set instant to now
            else
            {
                Instant = DateTimeOffset.Now;
            }
        }

        private void UpdateGamma()
        {
            // If update is not needed - return
            if (CurrentColorTemperature == TargetColorTemperature)
                return;

            // Determine if gamma change should be smooth
            var isSmooth = _settingsService.IsGammaSmoothingEnabled && !IsCyclePreviewEnabled;

            // If smooth - advance towards target temperature in small steps
            if (isSmooth)
            {
                // Calculate difference
                var diff = TargetColorTemperature.Value - CurrentColorTemperature.Value;

                // Calculate delta
                var delta = 30.0 * Math.Sign(diff);
                if (Math.Abs(delta) > Math.Abs(diff))
                    delta = diff;

                // Set new color temperature
                CurrentColorTemperature = new ColorTemperature(CurrentColorTemperature.Value + delta);
            }
            // Otherwise - just snap to target temperature
            else
            {
                CurrentColorTemperature = TargetColorTemperature;
            }

            // Update gamma
            _systemService.SetGamma(CurrentColorTemperature);
        }

        public void Enable() => IsEnabled = true;

        public void Disable() => IsEnabled = false;

        public void DisableTemporarily(TimeSpan duration)
        {
            // Schedule to enable after delay
            _enableAfterDelayTimer.Start(duration);

            // Disable
            IsEnabled = false;
        }

        public void NavigateGeneralSettings() => SettingsIndex = 0;

        public void NavigateLocationSettings() => SettingsIndex = 1;

        public void NavigateAdvancedSettings() => SettingsIndex = 2;

        public void ShowAbout() => Process.Start("https://github.com/Tyrrrz/LightBulb");

        public void ShowReleases() => Process.Start("https://github.com/Tyrrrz/LightBulb/releases");

        public void Exit()
        {
            // Save settings
            _settingsService.SaveIfNeeded();

            // Close
            RequestClose();
        }

        public void Handle(ToggleIsEnabledMessage message) => IsEnabled = !IsEnabled;

        public void Dispose()
        {
            // Dispose stuff
            _updateTimer.Dispose();
            _settingsAutoSaveTimer.Dispose();
            _internetSyncTimer.Dispose();
            _checkForUpdatesTimer.Dispose();
            _enableAfterDelayTimer.Dispose();

            // Reset gamma
            _systemService.SetGamma(ColorTemperature.Default);
        }
    }
}