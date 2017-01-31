using System;
using System.Diagnostics;
using LightBulb.Models;
using LightBulb.Services.Helpers;
using LightBulb.Services.Interfaces;
using Microsoft.Win32;
using Tyrrrz.Extensions;

namespace LightBulb.Services
{
    public class DefaultTemperatureService : ITemperatureService, IDisposable
    {
        private readonly ISettingsService _settingsService;
        private readonly IGammaService _gammaService;

        private readonly Timer _pollingTimer;
        private readonly SyncedTimer _temperatureUpdateTimer;
        private readonly Timer _cyclePreviewTimer;
        private readonly ValueSmoother _temperatureSmoother;

        private ushort _temperature;
        private bool _isRealtimeModeEnabled;
        private bool _isPreviewModeEnabled;
        private DateTime _cyclePreviewTime;
        private ushort _requestedPreviewTemperature;

        /// <inheritdoc />
        public ushort Temperature
        {
            get { return _temperature; }
            private set
            {
                if (Temperature == value) return;
                ushort oldTemp = _temperature;
                _temperature = value;

                Debug.WriteLine($"Updated temperature ({oldTemp} -> {value})", GetType().Name);
                UpdateGamma();

                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public bool IsRealtimeModeEnabled
        {
            get { return _isRealtimeModeEnabled; }
            set
            {
                if (IsRealtimeModeEnabled == value) return;
                _isRealtimeModeEnabled = value;

                _temperatureUpdateTimer.IsEnabled = value && !IsPreviewModeEnabled;
                _pollingTimer.IsEnabled = (value || IsPreviewModeEnabled) && _settingsService.IsGammaPollingEnabled;
                Debug.WriteLine($"Realtime mode {(value ? "enabled" : "disabled")}", GetType().Name);
                UpdateTemperature();

                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public bool IsPreviewModeEnabled
        {
            get { return _isPreviewModeEnabled; }
            set
            {
                if (IsPreviewModeEnabled == value) return;
                _isPreviewModeEnabled = value;

                _temperatureUpdateTimer.IsEnabled = !value && IsRealtimeModeEnabled;
                _pollingTimer.IsEnabled = (value || IsRealtimeModeEnabled) && _settingsService.IsGammaPollingEnabled;
                Debug.WriteLine($"Preview mode {(value ? "enabled" : "disabled")}", GetType().Name);
                UpdateTemperature(true);

                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public DateTime CyclePreviewTime
        {
            get { return _cyclePreviewTime; }
            private set
            {
                if (CyclePreviewTime == value) return;
                _cyclePreviewTime = value;

                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public bool IsCyclePreviewRunning => _cyclePreviewTimer.IsEnabled;

        /// <inheritdoc />
        public event EventHandler Updated;

        /// <inheritdoc />
        public event EventHandler CyclePreviewStarted;

        /// <inheritdoc />
        public event EventHandler CyclePreviewEnded;

        public DefaultTemperatureService(ISettingsService settingsService, IGammaService gammaService)
        {
            _settingsService = settingsService;
            _gammaService = gammaService;

            // Polling timer
            _pollingTimer = new Timer();
            _pollingTimer.Tick += (sender, args) => UpdateGamma();

            // Temperature update timer
            _temperatureUpdateTimer = new SyncedTimer();
            _temperatureUpdateTimer.Tick += (sender, args) =>
            {
                if (!_temperatureSmoother.IsActive)
                    UpdateTemperature(true);
            };

            // Cycle preview timer
            _cyclePreviewTimer = new Timer(TimeSpan.FromMilliseconds(15));
            _cyclePreviewTimer.Tick += (sender, args) =>
            {
                CyclePreviewTime = CyclePreviewTime.Add(TimeSpan.FromHours(0.05));
                IsPreviewModeEnabled = true;
                UpdateTemperature(true);

                // Ending condition
                if ((CyclePreviewTime - DateTime.Now).TotalHours >= 24)
                {
                    _cyclePreviewTimer.IsEnabled = false;
                    IsPreviewModeEnabled = false;
                    Debug.WriteLine("Ended cycle preview", GetType().Name);
                    CyclePreviewEnded?.Invoke(this, EventArgs.Empty);
                }
            };

            // Helpers
            _temperatureSmoother = new ValueSmoother();
            _temperatureSmoother.Finished += (sender, args) => UpdateTemperature(true); // snap

            // System events
            SystemEvents.PowerModeChanged += SystemPowerModeChanged;
            SystemEvents.DisplaySettingsChanged += SystemDisplaySettingsChanged;

            // Settings
            _settingsService.PropertyChanged += (sender, args) =>
            {
                UpdateConfiguration();

                if (args.PropertyName.IsEither(nameof(_settingsService.TemperatureEpsilon),
                    nameof(_settingsService.MinTemperature),
                    nameof(_settingsService.MaxTemperature), nameof(_settingsService.TemperatureTransitionDuration),
                    nameof(_settingsService.SunriseTime), nameof(_settingsService.SunsetTime)))
                {
                    if (!_temperatureSmoother.IsActive)
                        UpdateTemperature(true);
                }
            };
            UpdateConfiguration();

            // Init
            _temperature = _settingsService.DefaultMonitorTemperature;
        }

        private void SystemDisplaySettingsChanged(object sender, EventArgs e)
        {
            UpdateTemperature();
        }

        private void SystemPowerModeChanged(object sender, PowerModeChangedEventArgs powerModeChangedEventArgs)
        {
            UpdateTemperature();
        }

        private void UpdateConfiguration()
        {
            _temperatureUpdateTimer.Interval = _settingsService.TemperatureUpdateInterval;

            _pollingTimer.Interval = _settingsService.GammaPollingInterval;
            _pollingTimer.IsEnabled = (IsRealtimeModeEnabled || IsPreviewModeEnabled) &&
                                      _settingsService.IsGammaPollingEnabled;
        }

        private void UpdateGamma()
        {
            var intens = ColorIntensity.FromTemperature(Temperature);
            _gammaService.SetDisplayGammaLinear(intens);
            Debug.WriteLine($"Gamma updated (-> {intens})", GetType().Name);
        }

        private ushort GetTemperature(TimeSpan time)
        {
            ushort minTemp = _settingsService.MinTemperature;
            ushort maxTemp = _settingsService.MaxTemperature;

            var offset = _settingsService.TemperatureTransitionDuration;
            var halfOffset = TimeSpan.FromHours(offset.TotalHours/2);
            var riseTime = _settingsService.SunriseTime;
            var setTime = _settingsService.SunsetTime;
            var riseStartTime = riseTime - halfOffset;
            var riseEndTime = riseTime + halfOffset;
            var setStartTime = setTime - halfOffset;
            var setEndTime = setTime + halfOffset;

            // Before sunrise (night time)
            if (time < riseStartTime)
                return minTemp;

            // Incoming sunrise (night time -> day time)
            if (time >= riseStartTime && time <= riseEndTime)
            {
                double t = (time.TotalHours - riseStartTime.TotalHours)/offset.TotalHours;
                return (ushort) (minTemp + (maxTemp - minTemp)*Math.Sin(t*Math.PI/2));
            }

            // Between sunrise and sunset (day time)
            if (time > riseEndTime && time < setStartTime)
                return maxTemp;

            // Incoming sunset (day time -> night time)
            if (time >= setStartTime && time <= setEndTime)
            {
                double t = (time.TotalHours - setStartTime.TotalHours)/offset.TotalHours;
                return (ushort) (maxTemp + (minTemp - maxTemp)*Math.Sin(t*Math.PI/2));
            }

            // After sunset (night time)
            return minTemp;
        }

        private ushort GetTemperature(DateTime dt) => GetTemperature(dt.TimeOfDay);

        private void UpdateTemperature(bool forceInstantSwitch = false)
        {
            ushort newTemp;

            // 24 hr cycle preview mode
            if (IsPreviewModeEnabled && IsCyclePreviewRunning)
            {
                newTemp = GetTemperature(CyclePreviewTime);
            }
            // Preview mode
            else if (IsPreviewModeEnabled)
            {
                newTemp = _requestedPreviewTemperature;
            }
            // Realtime mode
            else
            {
                newTemp = IsRealtimeModeEnabled
                    ? GetTemperature(DateTime.Now)
                    : _settingsService.DefaultMonitorTemperature;
            }

            // Delta
            int delta = Math.Abs(newTemp - Temperature);

            // No change - nothing to do
            if (delta <= 0)
            {
                //Debug.WriteLine("Temperature delta is zero", GetType().Name);
                return;
            }

            // Don't update if delta is too small
            if (delta < _settingsService.TemperatureEpsilon &&
                !newTemp.IsEither(_settingsService.MinTemperature, _settingsService.MaxTemperature))
            {
                Debug.WriteLine($"Temperature delta too small to update ({delta})", GetType().Name);
                return;
            }

            // If allowed - start smooth transition
            if (!forceInstantSwitch && _settingsService.IsTemperatureSmoothingEnabled &&
                delta > _settingsService.TemperatureEpsilon)
            {
                double deltaNorm =
                    ((double) delta/(_settingsService.MaxTemperature - _settingsService.MinTemperature)).Clamp(0, 1);
                var duration =
                    TimeSpan.FromMilliseconds(_settingsService.MaximumTemperatureSmoothingDuration.TotalMilliseconds*
                                              deltaNorm);

                _temperatureSmoother.Set(
                    Temperature, newTemp,
                    temp => Temperature = (ushort) temp,
                    duration);

                Debug.WriteLine(
                    $"Started temperature transition ({Temperature} -> {newTemp}; over {duration.TotalSeconds:0.##}s)",
                    GetType().Name);
            }
            // Otherwise - instant transition
            else
            {
                // Stop existing smooth transition
                _temperatureSmoother.Stop();

                // Set temperature
                Temperature = newTemp;
            }
        }

        /// <inheritdoc />
        public void RequestPreviewTemperature(ushort temp)
        {
            _requestedPreviewTemperature = temp;
            if (IsPreviewModeEnabled)
                UpdateTemperature(true);
        }

        /// <inheritdoc />
        public void StartCyclePreview()
        {
            CyclePreviewTime = DateTime.Now;
            _cyclePreviewTimer.IsEnabled = true;

            Debug.WriteLine("Started cycle preview", GetType().Name);
            CyclePreviewStarted?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void StopCyclePreview()
        {
            _cyclePreviewTimer.IsEnabled = false;
            IsPreviewModeEnabled = false;

            Debug.WriteLine("Canceled cycle preview", GetType().Name);
            CyclePreviewEnded?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Dispose()
        {
            SystemEvents.PowerModeChanged -= SystemPowerModeChanged;
            SystemEvents.DisplaySettingsChanged -= SystemDisplaySettingsChanged;

            _temperatureUpdateTimer.Dispose();
            _cyclePreviewTimer.Dispose();
            _pollingTimer.Dispose();
            _temperatureSmoother.Dispose();
        }
    }
}