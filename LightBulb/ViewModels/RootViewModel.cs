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

        private readonly Timer _instantTimer;
        private readonly DelayedActionScheduler _disableTemporarilyScheduler = new DelayedActionScheduler();
        private readonly Sequence _previewSequence;

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

            // Initialize update timer
            _instantTimer = new Timer(TimeSpan.FromSeconds(5), UpdateInstant);

            _previewSequence = new Sequence(TimeSpan.FromMilliseconds(50), (int) Math.Ceiling(24 / 0.05),
                () =>
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
                    _disableTemporarilyScheduler.Unschedule();
            });

            // When disabled - reset gamma
            this.Bind(o => o.IsEnabled, (sender, args) =>
            {
                if (!args.NewValue)
                    _gammaService.ResetGamma();
            });
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
            _disableTemporarilyScheduler.Schedule(duration, () => IsEnabled = true);

            // Disable
            IsEnabled = false;
        }

        public void Preview()
        {
            _previewSequence.Start();
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
            _disableTemporarilyScheduler.Dispose();
        }
    }
}