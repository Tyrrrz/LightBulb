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
        private readonly AutoResetTimer _cyclePreviewUpdateTimer;
        private readonly ManualResetTimer _enableAfterDelayTimer;

        public string ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public bool IsEnabled { get; set; } = true;

        public bool IsCyclePreviewEnabled { get; private set; }

        public DateTimeOffset CurrentInstant { get; private set; } = DateTimeOffset.Now;

        public ColorTemperature CurrentColorTemperature => _colorTemperatureService.GetTemperature(CurrentInstant);

        public double CurrentSolarCyclePosition => CurrentInstant.TimeOfDay.TotalDays;

        public SolarCycleState CurrentSolarCycleState => !IsEnabled ? SolarCycleState.Disabled :
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

            // Initialize timers
            _updateTimer = new AutoResetTimer(UpdateTick);
            _cyclePreviewUpdateTimer = new AutoResetTimer(CyclePreviewUpdateTick);
            _enableAfterDelayTimer = new ManualResetTimer(Enable);

            // Bind on IsEnabled changes
            this.Bind(o => o.IsEnabled, (sender, args) =>
            {
                if (IsEnabled)
                {
                    _enableAfterDelayTimer.Stop();
                    _gammaService.SetGamma(CurrentColorTemperature);
                }
                else
                {
                    _gammaService.ResetGamma();
                }
            });
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            // Load settings
            _settingsService.Load();

            // Start update timer
            _updateTimer.Start(TimeSpan.FromSeconds(5));
        }

        private void UpdateTick()
        {
            // If disabled - return
            if (!IsEnabled)
                return;

            // If cycle preview is currently running - return
            if (IsCyclePreviewEnabled)
                return;

            // Set new instant
            CurrentInstant = DateTimeOffset.Now;

            // Update gamma
            _gammaService.SetGamma(CurrentColorTemperature);
        }

        private void CyclePreviewUpdateTick()
        {
            // Advance current instant by this much on every tick
            var delta = TimeSpan.FromMinutes(3);

            // If current instant is 1 day (full cycle) past real current instant - stop
            if (CurrentInstant >= DateTimeOffset.Now + TimeSpan.FromDays(1) - delta)
            {
                StopCyclePreview();
            }
            // Otherwise - shift current instant by delta
            else
            {
                CurrentInstant += delta;
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
            Disable();
        }

        public void StartCyclePreview()
        {
            _cyclePreviewUpdateTimer.Start(TimeSpan.FromMilliseconds(17));
            IsCyclePreviewEnabled = true;
        }

        public void StopCyclePreview()
        {
            _cyclePreviewUpdateTimer.Stop();
            IsCyclePreviewEnabled = false;

            // Reset instant and update gamma
            CurrentInstant = DateTimeOffset.Now;
            _gammaService.SetGamma(CurrentColorTemperature);
        }

        public void ShowAbout() => Process.Start("https://github.com/Tyrrrz/LightBulb");

        public void ShowReleases() => Process.Start("https://github.com/Tyrrrz/LightBulb/releases");

        public void Exit() => RequestClose();

        public void Dispose()
        {
            // Reset gamma
            _gammaService.ResetGamma();

            // Dispose stuff
            _updateTimer.Dispose();
            _enableAfterDelayTimer.Dispose();
            _cyclePreviewUpdateTimer.Dispose();
        }
    }
}