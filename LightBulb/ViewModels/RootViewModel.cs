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

        private readonly AutoResetTimer _gammaUpdateTimer;
        private readonly AutoResetTimer _instantUpdateTimer;
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

        public double SolarCyclePosition => Instant.TimeOfDay.TotalDays;

        public SolarCycleState SolarCycleState => !IsEnabled ? SolarCycleState.Disabled :
            CurrentColorTemperature == _settingsService.MaxTemperature ? SolarCycleState.Day :
            CurrentColorTemperature == _settingsService.MinTemperature ? SolarCycleState.Night :
            SolarCycleState.Transition;

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
            _gammaUpdateTimer = new AutoResetTimer(GammaUpdateTick);
            _instantUpdateTimer = new AutoResetTimer(InstantUpdateTick);
            _enableAfterDelayTimer = new ManualResetTimer(Enable);
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            // Load settings
            _settingsService.Load();

            // Start timers
            _gammaUpdateTimer.Start(TimeSpan.FromMilliseconds(17));
            _instantUpdateTimer.Start(TimeSpan.FromMilliseconds(17));
        }

        private void GammaUpdateTick()
        {
            // If reached target temperature - return
            if (CurrentColorTemperature == TargetColorTemperature)
                return;

            // Determine if temperature transition should be smooth
            var isSmooth = !IsCyclePreviewEnabled;

            // If smooth - advance towards target temperature in small steps
            if (isSmooth)
            {
                // Calculate difference and delta
                var diff = TargetColorTemperature.Value - CurrentColorTemperature.Value;
                var delta = 30 * Math.Sign(diff);

                // Calculate new color temperature
                CurrentColorTemperature = Math.Abs(diff) >= Math.Abs(delta)
                    ? new ColorTemperature(CurrentColorTemperature.Value + delta)
                    : TargetColorTemperature;
            }
            // Otherwise - just snap to target temperature
            else
            {
                CurrentColorTemperature = TargetColorTemperature;
            }

            // Update gamma
            _gammaService.SetGamma(CurrentColorTemperature);
        }

        private void InstantUpdateTick()
        {
            // If disabled - return
            if (!IsEnabled)
                return;

            // If in cycle preview mode - advance quickly until reached full cycle
            if (IsCyclePreviewEnabled)
            {
                // Expected delta
                var delta = TimeSpan.FromMinutes(3);

                // Cycle is supposed to end 1 full day past current real time
                var cycleEnd = DateTimeOffset.Now + TimeSpan.FromDays(1);

                // If end of cycle is within delta - snap to the end and disable cycle preview
                if (cycleEnd - Instant <= delta)
                {
                    Instant = cycleEnd;
                    IsCyclePreviewEnabled = false;
                }
                // Otherwise - advance by delta
                else
                {
                    Instant += delta;
                }
            }
            // Otherwise - simply update instant with current time
            else
            {
                Instant = DateTimeOffset.Now;
            }
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
            _gammaUpdateTimer.Dispose();
            _instantUpdateTimer.Dispose();
            _enableAfterDelayTimer.Dispose();

            // Reset gamma
            _gammaService.SetGamma(ColorTemperature.Default);
        }
    }
}