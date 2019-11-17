using System;
using System.Threading.Tasks;
using LightBulb.Calculators;
using LightBulb.Internal;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.ViewModels.Framework;
using Stylet;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels
{
    public class RootViewModel : Screen, IDisposable
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly DialogManager _dialogManager;
        private readonly SettingsService _settingsService;
        private readonly UpdateService _updateService;
        private readonly SystemService _systemService;

        private readonly AutoResetTimer _updateTimer;
        private readonly AutoResetTimer _checkForUpdatesTimer;
        private readonly ManualResetTimer _enableAfterDelayTimer;

        public bool IsEnabled { get; set; } = true;

        public bool IsPaused { get; private set; }

        public bool IsCyclePreviewEnabled { get; set; }

        public bool IsWorking => IsEnabled && !IsPaused || IsCyclePreviewEnabled;

        public DateTimeOffset Instant { get; private set; } = DateTimeOffset.Now;

        public TimeSpan ActualSunriseTime => _settingsService.Location != null && !_settingsService.IsManualSunriseSunsetEnabled
            ? Astronomy.CalculateSunriseTime(_settingsService.Location.Value, Instant)
            : _settingsService.ManualSunriseTime;

        public TimeSpan ActualSunsetTime => _settingsService.Location != null && !_settingsService.IsManualSunriseSunsetEnabled
            ? Astronomy.CalculateSunsetTime(_settingsService.Location.Value, Instant)
            : _settingsService.ManualSunsetTime;

        public TimeSpan ActualConfigurationTransitionDuration =>
            _settingsService.ConfigurationTransitionDuration.ClampMax((ActualSunsetTime - ActualSunriseTime).Duration());

        public TimeSpan ActualSunriseEndTime => ActualSunriseTime + ActualConfigurationTransitionDuration;

        public TimeSpan ActualSunsetEndTime => ActualSunsetTime + ActualConfigurationTransitionDuration;

        public ColorConfiguration TargetColorConfiguration
        {
            get
            {
                // If working - calculate color configuration for current instant
                if (IsWorking)
                {
                    return Flow.CalculateColorConfiguration(
                        ActualSunriseTime, _settingsService.DayConfiguration,
                        ActualSunsetTime, _settingsService.NightConfiguration,
                        ActualConfigurationTransitionDuration, Instant);
                }

                // Otherwise - return default configuration
                return _settingsService.IsDefaultToDayConfigurationEnabled
                    ? _settingsService.DayConfiguration
                    : ColorConfiguration.Default;
            }
        }

        public ColorConfiguration CurrentColorConfiguration { get; private set; } = ColorConfiguration.Default;

        public CycleState CycleState
        {
            get
            {
                // If target configuration has not been reached - return transition (even when disabled)
                if (CurrentColorConfiguration != TargetColorConfiguration)
                    return CycleState.Transition;

                // If disabled or paused - return disabled
                if (!IsWorking)
                    return CycleState.Disabled;

                // If at day configuration - return day
                if (CurrentColorConfiguration == _settingsService.DayConfiguration)
                    return CycleState.Day;

                // If at night configuration - return night
                if (CurrentColorConfiguration == _settingsService.NightConfiguration)
                    return CycleState.Night;

                // Otherwise - return transition (shouldn't reach here)
                return CycleState.Transition;
            }
        }

        public RootViewModel(IViewModelFactory viewModelFactory, DialogManager dialogManager,
            SettingsService settingsService, UpdateService updateService, SystemService systemService)
        {
            _viewModelFactory = viewModelFactory;
            _dialogManager = dialogManager;
            _settingsService = settingsService;
            _updateService = updateService;
            _systemService = systemService;

            // Title
            DisplayName = $"{App.Name} v{App.VersionString}";

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
            var dialog = _viewModelFactory.CreateMessageBoxViewModel("Limited gamma range", 
                $"{App.Name} detected that this computer doesn't have the extended gamma range unlocked. " +
                "This may cause the app to work incorrectly with some settings." +
                Environment.NewLine + Environment.NewLine +
                "Press OK to unlock gamma range.",
                "OK", "CANCEL");

            var promptResult = await _dialogManager.ShowDialogAsync(dialog);

            // Unlock gamma range if user agreed to it
            if (promptResult == true)
                _systemService.UnlockGammaRange();
        }

        private async Task ShowFirstTimeExperienceMessageAsync()
        {
            if (!_settingsService.IsFirstTimeExperienceEnabled)
                return;

            // Show message to the user
            var dialog = _viewModelFactory.CreateMessageBoxViewModel("Welcome",
                $"Thank you for installing {App.Name}!" +
                Environment.NewLine + Environment.NewLine +
                "To get the most personalized experience, configure your location in settings.",
                "OK", null);

            await _dialogManager.ShowDialogAsync(dialog);

            // Disable first time experience
            _settingsService.IsFirstTimeExperienceEnabled = false;
            _settingsService.Save();
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            // Load settings
            _settingsService.Load();

            // Register hot keys
            RegisterHotKeys();

            // Register callback for monitor power on
            DisplayStateChangeEvent();

            // Refresh
            Refresh();

            // Start timers
            _updateTimer.Start(TimeSpan.FromMilliseconds(50));
            _checkForUpdatesTimer.Start(TimeSpan.FromHours(3));
        }
        
        // This is a custom event that fires when the dialog host is loaded
        public async void OnViewFullyLoaded()
        {
            await EnsureGammaRangeIsUnlockedAsync();
            await ShowFirstTimeExperienceMessageAsync();
        }

        private void RegisterHotKeys()
        {
            _systemService.UnregisterAllHotKeys();

            if (_settingsService.ToggleHotKey != HotKey.None)
            {
                _systemService.RegisterHotKey(_settingsService.ToggleHotKey, Toggle);
            }
        }

        private void DisplayStateChangeEvent()
        {
            _systemService.UnregisterPowerEvents();
            _systemService.RegisterPowerEvent(
                new Guid("6FE69556-704A-47A0-8F24-C28D936FDA47"),
                DisplayStateChange);
        }

        private void DisplayStateChange(byte newState)
        {
            // Only force update if display is turning on
            if(newState == 0x1)
                UpdateGamma(true);
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

                // Update instant
                Instant = Instant.StepTo(targetInstant, TimeSpan.FromMinutes(5));

                // If target instant reached - disable cycle preview
                if (Instant >= targetInstant)
                    IsCyclePreviewEnabled = false;
            }
            // Otherwise - synchronize instant with system clock
            else
            {
                Instant = DateTimeOffset.Now;
            }
        }

        private void UpdateGamma(bool force = false)
        {
            // Don't update if already reached target
            if (!force && CurrentColorConfiguration == TargetColorConfiguration)
                return;

            // Don't update on small changes to avoid lag
            var isSmallChange =
                Math.Abs(TargetColorConfiguration.Temperature - CurrentColorConfiguration.Temperature) < 25 &&
                Math.Abs(TargetColorConfiguration.Brightness - CurrentColorConfiguration.Brightness) < 0.01;

            var isExtremeState =
                TargetColorConfiguration == _settingsService.NightConfiguration ||
                TargetColorConfiguration == _settingsService.DayConfiguration;

            if (!force && isSmallChange && !isExtremeState)
                return;

            // Update current configuration
            var isSmooth = _settingsService.IsGammaSmoothingEnabled && !IsCyclePreviewEnabled;

            CurrentColorConfiguration = isSmooth
                ? CurrentColorConfiguration.StepTo(TargetColorConfiguration, 30, 0.008)
                : TargetColorConfiguration;

            // Set gamma to new value
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

        public void DisableTemporarilyUntilSunrise()
        {
            // Use real time here instead of Instant, because that's what the user likely wants
            var timeUntilSunrise = DateTimeOffset.Now.NextTimeOfDay(ActualSunriseEndTime) - DateTimeOffset.Now;
            DisableTemporarily(timeUntilSunrise);
        }

        public void Toggle() => IsEnabled = !IsEnabled;

        public void EnableCyclePreview() => IsCyclePreviewEnabled = true;

        public void DisableCyclePreview() => IsCyclePreviewEnabled = false;

        public async void ShowSettings()
        {
            await _dialogManager.ShowDialogAsync(_viewModelFactory.CreateSettingsViewModel());

            // Re-register hot keys
            RegisterHotKeys();

            // Refresh
            Refresh();
        }

        public void ShowAbout() => App.GitHubProjectUrl.ToUri().OpenInBrowser();

        public void Exit() => RequestClose();

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