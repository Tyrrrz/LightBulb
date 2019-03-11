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

        private readonly Timer _updateTimer;
        private readonly DelayedActionController _delayedEnableController = new DelayedActionController();

        public string ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public bool IsEnabled { get; set; }

        public ColorTemperature CurrentColorTemperature { get; private set; }

        public double CurrentCyclePosition { get; private set; }

        public SolarCycleState CurrentSolarCycleState { get; private set; }

        public RootViewModel(IViewModelFactory viewModelFactory, SettingsService settingsService,
            ColorTemperatureService colorTemperatureService, GammaService gammaService)
        {
            _viewModelFactory = viewModelFactory;
            _settingsService = settingsService;
            _colorTemperatureService = colorTemperatureService;
            _gammaService = gammaService;

            // Initialize update timer
            _updateTimer = new Timer(TimeSpan.FromSeconds(1), UpdateTick);

            // When enabled - reset 'disable temporarily'
            this.Bind(o => o.IsEnabled, (sender, args) =>
            {
                if (args.NewValue)
                    _delayedEnableController.Unschedule();
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

        private void UpdateTick()
        {
            // Don't do anything if disabled
            if (!IsEnabled)
                return;

            // Get instant
            var instant = DateTimeOffset.Now;

            // Get new color temperature
            var newColorTemperature = _colorTemperatureService.GetTemperature(instant);

            // If the temperature changed, update gamma
            if (CurrentColorTemperature != newColorTemperature)
            {
                CurrentColorTemperature = newColorTemperature;
                _gammaService.SetGamma(CurrentColorTemperature);
            }

            // Update cycle position
            CurrentCyclePosition = instant.TimeOfDay.TotalDays;

            // Update cycle state
            CurrentSolarCycleState = !IsEnabled ? SolarCycleState.Disabled :
                CurrentColorTemperature == _settingsService.MaxTemperature ? SolarCycleState.Day :
                CurrentColorTemperature == _settingsService.MinTemperature ? SolarCycleState.Night :
                SolarCycleState.Transition;
        }

        public void ToggleIsEnabled() => IsEnabled = !IsEnabled;

        public void DisableTemporarily(TimeSpan duration)
        {
            // Schedule to enable after delay
            _delayedEnableController.Schedule(duration, () => IsEnabled = true);

            // Disable
            IsEnabled = false;
        }

        public void ShowAbout() => Process.Start("https://github.com/Tyrrrz/LightBulb");

        public void ShowReleases() => Process.Start("https://github.com/Tyrrrz/LightBulb/releases");

        public void Exit() => RequestClose();

        public void Dispose()
        {
            // Reset gamma
            _gammaService.ResetGamma();

            // Dispose stuff
            _gammaService.Dispose();
            _updateTimer.Dispose();
            _delayedEnableController.Dispose();
        }
    }
}