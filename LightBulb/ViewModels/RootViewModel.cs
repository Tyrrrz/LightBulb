using System;
using System.Diagnostics;
using System.Reflection;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.ViewModels.Framework;
using LightBulb.Timers;
using LightBulb.ViewModels.Components;
using Stylet;

namespace LightBulb.ViewModels
{
    public class RootViewModel : Screen, IDisposable
    {
        private readonly SettingsService _settingsService;
        private readonly ColorTemperatureService _colorTemperatureService;
        private readonly GammaService _gammaService;

        private readonly AutoResetTimer _updateTimer;
        private readonly AutoResetTimer _checkForUpdatesTimer;
        private readonly ManualResetTimer _enableAfterDelayTimer;

        public string ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public bool IsUpdateAvailable { get; private set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsCyclePreviewEnabled { get; set; }

        public DateTimeOffset Instant { get; private set; } = DateTimeOffset.Now;

        public ColorTemperature TargetColorTemperature =>
            IsEnabled || IsCyclePreviewEnabled
                ? _colorTemperatureService.GetTemperature(Instant)
                : ColorTemperature.Default;

        public ColorTemperature CurrentColorTemperature { get; private set; } = ColorTemperature.Default;

        public double CyclePosition => Instant.TimeOfDay.TotalDays;

        public CycleState CycleState
        {
            get
            {
                // If disabled, not in cycle preview, and target temperature reached - disabled
                if (!IsEnabled && !IsCyclePreviewEnabled && CurrentColorTemperature == TargetColorTemperature)
                    return CycleState.Disabled;

                // If target temperature has not been reached - in transition
                if (CurrentColorTemperature != TargetColorTemperature)
                    return CycleState.Transition;

                // If at max temperature - day
                if (CurrentColorTemperature == _settingsService.MaxTemperature)
                    return CycleState.Day;

                // If at min temperature - night
                if (CurrentColorTemperature == _settingsService.MinTemperature)
                    return CycleState.Night;

                // Otherwise - in transition (shouldn't reach here)
                return CycleState.Transition;
            }
        }

        public string StatusText
        {
            get
            {
                // If in cycle preview - show current temperature and instant
                if (IsCyclePreviewEnabled)
                    return $"Temp: {CurrentColorTemperature}   Time: {Instant:t}";

                // If enabled or in transition - show current temperature
                if (IsEnabled || CurrentColorTemperature != TargetColorTemperature)
                    return $"Temp: {CurrentColorTemperature}";

                // Otherwise - disabled
                return "Disabled";
            }
        }

        public GeneralSettingsViewModel GeneralSettings { get; }

        public LocationSettingsViewModel LocationSettings { get; }

        public int SettingsIndex { get; set; }

        public RootViewModel(IViewModelFactory viewModelFactory, SettingsService settingsService,
            UpdateService updateService, ColorTemperatureService colorTemperatureService, GammaService gammaService)
        {
            _settingsService = settingsService;
            _colorTemperatureService = colorTemperatureService;
            _gammaService = gammaService;

            // Initialize view models
            GeneralSettings = viewModelFactory.CreateGeneralSettingsViewModel();
            LocationSettings = viewModelFactory.CreateLocationSettingsViewModel();

            // When IsEnabled switches to 'true' - cancel 'disable temporarily'
            this.Bind(o => o.IsEnabled, (sender, args) =>
            {
                if (IsEnabled)
                    _enableAfterDelayTimer.Stop();
            });

            // Initialize timers
            _updateTimer = new AutoResetTimer(() =>
            {
                UpdateInstant();
                UpdateGamma();
            });

            _checkForUpdatesTimer = new AutoResetTimer(async () =>
            {
                IsUpdateAvailable = await updateService.CheckForUpdatesAsync();
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
            _checkForUpdatesTimer.Start(TimeSpan.FromHours(3));
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

                // Calculate delta delta
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
            // If reached target temperature - return
            if (CurrentColorTemperature == TargetColorTemperature)
                return;

            // Determine if temperature transition should be smooth
            var isSmooth = !IsCyclePreviewEnabled;

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
            _gammaService.SetGamma(CurrentColorTemperature);
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

        public void ShowAbout() => Process.Start("https://github.com/Tyrrrz/LightBulb");

        public void ShowReleases() => Process.Start("https://github.com/Tyrrrz/LightBulb/releases");

        public void Exit() => RequestClose();

        public void Dispose()
        {
            // Dispose stuff
            _updateTimer.Dispose();
            _enableAfterDelayTimer.Dispose();

            // Reset gamma
            _gammaService.SetGamma(ColorTemperature.Default);
        }
    }
}