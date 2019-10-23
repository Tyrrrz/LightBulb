using System;
using System.Threading.Tasks;
using LightBulb.Helpers;
using LightBulb.Internal;
using LightBulb.Messages;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.Timers;
using LightBulb.ViewModels.Framework;
using Stylet;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels
{
    public class RootViewModel : Screen, IHandle<ToggleIsEnabledMessage>, IDisposable
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly DialogManager _dialogManager;
        private readonly SettingsService _settingsService;
        private readonly UpdateService _updateService;
        private readonly CalculationService _calculationService;
        private readonly SystemService _systemService;

        private readonly AutoResetTimer _updateTimer;
        private readonly AutoResetTimer _checkForUpdatesTimer;
        private readonly ManualResetTimer _enableAfterDelayTimer;

        public bool IsEnabled { get; set; } = true;

        public bool IsPaused { get; private set; }

        public bool IsCyclePreviewEnabled { get; set; }

        public DateTimeOffset Instant { get; private set; } = DateTimeOffset.Now;

        public ColorConfiguration TargetColorConfiguration
        {
            get
            {
                // If in cycle preview - return temperature for instant (even when disabled)
                if (IsCyclePreviewEnabled)
                    return _calculationService.CalculateColorTemperature(Instant);

                // If disabled or paused - return default temperature
                if (!IsEnabled || IsPaused)
                {
                    return _settingsService.IsDefaultToDayConfigurationEnabled
                        ? new ColorConfiguration(_settingsService.DayTemperature, _settingsService.DayBrightness)
                        : ColorConfiguration.Default;
                }

                // Otherwise - return temperature for instant
                return _calculationService.CalculateColorTemperature(Instant);
            }
        }

        public ColorConfiguration CurrentColorConfiguration { get; private set; } = ColorConfiguration.Default;

        public CycleState CycleState
        {
            get
            {
                // If target temperature has not been reached - return transition (even when disabled)
                if (CurrentColorConfiguration != TargetColorConfiguration)
                    return CycleState.Transition;

                // If disabled or paused - return disabled
                if (!IsEnabled || IsPaused)
                    return CycleState.Disabled;

                // If at max temperature - return day
                if (CurrentColorConfiguration.Equals(_settingsService.DayTemperature, _settingsService.DayBrightness))
                    return CycleState.Day;

                // If at min temperature and enabled and not paused - return night
                if (CurrentColorConfiguration.Equals(_settingsService.NightTemperature, _settingsService.NightBrightness))
                    return CycleState.Night;

                // Otherwise - return transition (shouldn't reach here)
                return CycleState.Transition;
            }
        }

        public TimeSpan SunriseTime => _settingsService.IsManualSunriseSunsetEnabled || _settingsService.Location == null
            ? _settingsService.ManualSunriseTime
            : Astronomy.CalculateSunrise(_settingsService.Location.Value, Instant).TimeOfDay;

        public TimeSpan SunsetTime => _settingsService.IsManualSunriseSunsetEnabled || _settingsService.Location == null
            ? _settingsService.ManualSunsetTime
            : Astronomy.CalculateSunset(_settingsService.Location.Value, Instant).TimeOfDay;

        public TimeSpan TimeUntilSunrise => Instant.NextTimeOfDay(SunriseTime) - Instant;

        public TimeSpan TimeUntilSunset => Instant.NextTimeOfDay(SunsetTime) - Instant;

        public RootViewModel(
            IEventAggregator eventAggregator, IViewModelFactory viewModelFactory, DialogManager dialogManager,
            SettingsService settingsService, UpdateService updateService,
            CalculationService calculationService, SystemService systemService)
        {
            _viewModelFactory = viewModelFactory;
            _dialogManager = dialogManager;
            _settingsService = settingsService;
            _updateService = updateService;
            _calculationService = calculationService;
            _systemService = systemService;

            // Title
            DisplayName = $"{App.Name} v{App.VersionString}";

            // Handle messages
            eventAggregator.Subscribe(this);

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

            _checkForUpdatesTimer = new AutoResetTimer(async () =>
            {
                await _updateService.CheckPrepareUpdateAsync();
            });

            _enableAfterDelayTimer = new ManualResetTimer(Enable);
        }

        private async Task EnsureGammaRangeIsUnlockedAsync()
        {
            // If already unlocked - return
            if (_systemService.IsGammaRangeUnlocked())
                return;

            // Show prompt to the user
            var prompt = _viewModelFactory.CreateMessageBoxViewModel("Limited gamma range", 
                $"{App.Name} detected that this computer doesn't have the extended gamma range unlocked. " +
                $"This may cause the app to work incorrectly.{Environment.NewLine}{Environment.NewLine}" +
                "Press OK to unlock gamma range.");

            var promptResult = await _dialogManager.ShowDialogAsync(prompt);

            // Unlock gamma range if user agreed to it
            if (promptResult == true)
                _systemService.UnlockGammaRange();
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            // Load settings
            _settingsService.Load();

            // Start timers
            _updateTimer.Start(TimeSpan.FromMilliseconds(50));
            _checkForUpdatesTimer.Start(TimeSpan.FromHours(3));
        }
        
        // This is a custom event that fires when the dialog host is loaded
        public async void OnViewFullyLoaded()
        {
            await EnsureGammaRangeIsUnlockedAsync();
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
                var delta = TimeSpan.FromMinutes(5);
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
            if (CurrentColorConfiguration == TargetColorConfiguration)
                return;

            var isSmooth = _settingsService.IsGammaSmoothingEnabled && !IsCyclePreviewEnabled;

            CurrentColorConfiguration = isSmooth
                ? CurrentColorConfiguration.Interpolate(TargetColorConfiguration)
                : TargetColorConfiguration;

            _systemService.SetGamma(CurrentColorConfiguration);
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

        public void Toggle() => IsEnabled = !IsEnabled;

        public void EnableCyclePreview() => IsCyclePreviewEnabled = true;

        public void DisableCyclePreview() => IsCyclePreviewEnabled = false;

        public async void ShowSettings() => await _dialogManager.ShowDialogAsync(_viewModelFactory.CreateSettingsViewModel());

        public void ShowAbout() => App.GitHubProjectUrl.ToUri().OpenInBrowser();

        public void ShowReleases() => App.GitHubProjectReleasesUrl.ToUri().OpenInBrowser();

        public void Exit() => RequestClose();

        public void Handle(ToggleIsEnabledMessage message) => Toggle();

        public void Dispose()
        {
            // Dispose stuff
            _updateTimer.Dispose();
            _checkForUpdatesTimer.Dispose();
            _enableAfterDelayTimer.Dispose();

            // Reset gamma
            _systemService.SetGamma(ColorConfiguration.Default);
        }
    }
}