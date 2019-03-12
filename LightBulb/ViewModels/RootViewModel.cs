using System;
using System.Diagnostics;
using System.Reflection;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.ViewModels.Framework;
using LightBulb.WindowsApi;
using Stylet;

namespace LightBulb.ViewModels
{
    public class RootViewModel : Screen, IDisposable
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly SettingsService _settingsService;
        private readonly ColorTemperatureService _colorTemperatureService;
        private readonly GammaService _gammaService;

        private readonly AutoResetTimer _instantTimer;
        private readonly ManualResetTimer _enableAfterDelayTimer;
        private readonly FixedDurationAutoResetTimer _cyclePreviewTimer;

        public string ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public bool IsEnabled { get; set; } = true;

        public DateTimeOffset CurrentInstant { get; private set; } = DateTimeOffset.Now;

        public ColorTemperature CurrentColorTemperature => _colorTemperatureService.GetTemperature(CurrentInstant);

        public double CurrentSolarCyclePosition => IsEnabled ? CurrentInstant.TimeOfDay.TotalDays : 0;

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

            // Initialize stuff
            _instantTimer = new AutoResetTimer(UpdateInstant);
            _enableAfterDelayTimer = new ManualResetTimer(() => IsEnabled = true);
            _cyclePreviewTimer = new FixedDurationAutoResetTimer(TimeSpan.FromSeconds(6), () =>
            {
                var oldTemperature = CurrentColorTemperature;
                CurrentInstant = CurrentInstant.AddHours(0.05);
                if (oldTemperature != CurrentColorTemperature)
                    _gammaService.SetGamma(CurrentColorTemperature);
            });

            // When enabled - reset 'disable temporarily'
            this.Bind(o => o.IsEnabled, (sender, args) =>
            {
                if (args.NewValue)
                    _enableAfterDelayTimer.Stop();
            });

            // When disabled - reset gamma
            this.Bind(o => o.IsEnabled, (sender, args) =>
            {
                if (!args.NewValue)
                    _gammaService.ResetGamma();
            });
            
            // Start instant timer
            _instantTimer.Start(TimeSpan.FromTicks(5));
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            // Load settings
            _settingsService.Load();
        }

        private void UpdateInstant()
        {
            // Don't do anything if disabled
            if (!IsEnabled)
                return;

            // Set new instant
            //CurrentInstant = DateTimeOffset.Now;

            // Update gamma
            _gammaService.SetGamma(CurrentColorTemperature);
        }

        public void ToggleIsEnabled() => IsEnabled = !IsEnabled;

        public void DisableTemporarily(TimeSpan duration)
        {
            // Schedule to enable after delay
            _enableAfterDelayTimer.Start(duration);

            // Disable
            IsEnabled = false;
        }

        public void PreviewCycle()
        {
            _cyclePreviewTimer.Start();
        }

        public void ShowAbout() => Process.Start("https://github.com/Tyrrrz/LightBulb");

        public void ShowReleases() => Process.Start("https://github.com/Tyrrrz/LightBulb/releases");

        public void Exit() => RequestClose();

        public void Dispose()
        {
            // Reset gamma
            _gammaService.ResetGamma();

            // Dispose stuff
            _instantTimer.Dispose();
            _enableAfterDelayTimer.Dispose();
            _cyclePreviewTimer.Dispose();
        }
    }
}