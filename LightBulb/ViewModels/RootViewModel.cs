using System;
using System.Diagnostics;
using System.Reflection;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.ViewModels.Framework;
using LightBulb.Timers;
using Stylet;

namespace LightBulb.ViewModels
{
    public class RootViewModel : Screen, IDisposable
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly SettingsService _settingsService;
        private readonly ColorTemperatureService _colorTemperatureService;
        private readonly GammaService _gammaService;

        private readonly AutoResetTimer _updateTimer;
        private readonly ManualResetTimer _enableAfterDelayTimer;

        public string ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public bool IsEnabled { get; set; } = true;

        public bool IsCyclePreviewEnabled { get; private set; }

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

                // If at max temperature - day
                if (CurrentColorTemperature == _settingsService.MaxTemperature)
                    return CycleState.Day;

                // If at min temperature - night
                if (CurrentColorTemperature == _settingsService.MinTemperature)
                    return CycleState.Night;

                // Otherwise - in transition
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

        public RootViewModel(IViewModelFactory viewModelFactory, SettingsService settingsService,
            ColorTemperatureService colorTemperatureService, GammaService gammaService)
        {
            _viewModelFactory = viewModelFactory;
            _settingsService = settingsService;
            _colorTemperatureService = colorTemperatureService;
            _gammaService = gammaService;

            // When IsEnabled switches to 'true' - cancel 'disable temporarily'
            this.Bind(o => o.IsEnabled, (sender, args) =>
            {
                if (IsEnabled)
                    _enableAfterDelayTimer.Stop();
            });

            // Initialize timers
            _updateTimer = new AutoResetTimer(UpdateTick);
            _enableAfterDelayTimer = new ManualResetTimer(Enable);
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            // Load settings
            _settingsService.Load();

            // Start update timer at 60hz
            _updateTimer.Start(TimeSpan.FromMilliseconds(17));
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

        private void UpdateTick()
        {
            UpdateInstant();
            UpdateGamma();
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

        public void StartCyclePreview() => IsCyclePreviewEnabled = true;

        public void StopCyclePreview() => IsCyclePreviewEnabled = false;

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