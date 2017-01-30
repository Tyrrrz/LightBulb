using System;
using System.Diagnostics;
using LightBulb.Models;
using LightBulb.Services.Helpers;
using Microsoft.Win32;
using Tyrrrz.Extensions;

namespace LightBulb.Services
{
    public partial class TemperatureService : IDisposable
    {
        private readonly GammaService _gammaService;

        private readonly Timer _pollingTimer;
        private readonly SyncedTimer _temperatureUpdateTimer;
        private readonly Timer _cyclePreviewTimer;
        private readonly ValueSmoother _temperatureSmoother;

        private ushort _temperature;
        private bool _isRealtimeModeEnabled;
        private bool _isPreviewModeEnabled;
        private DateTime _cyclePreviewTime;
        private ushort _requestedPreviewTemperature;

        /// <summary>
        /// Current display color temperature
        /// </summary>
        public ushort Temperature
        {
            get { return _temperature; }
            private set
            {
                if (Temperature == value) return;
                int diff = Math.Abs(Temperature - value);
                if (diff <= Settings.TemperatureEpsilon &&
                    !value.IsEither(Settings.MaxTemperature, Settings.MinTemperature)) return;
                _temperature = value;

                Debug.WriteLine($"Updated temperature (to {value})", GetType().Name);

                UpdateGamma();

                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Whether the real time gamma control is enabled
        /// </summary>
        public bool IsRealtimeModeEnabled
        {
            get { return _isRealtimeModeEnabled; }
            set
            {
                if (IsRealtimeModeEnabled == value) return;
                _isRealtimeModeEnabled = value;

                _temperatureUpdateTimer.IsEnabled = value;
                _pollingTimer.IsEnabled = (value || IsPreviewModeEnabled) && Settings.IsGammaPollingEnabled;

                Debug.WriteLine($"Realtime mode {(value ? "enabled" : "disabled")}", GetType().Name);

                UpdateTemperature();
                UpdateGamma();

                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Whether preview mode is enabled
        /// </summary>
        public bool IsPreviewModeEnabled
        {
            get { return _isPreviewModeEnabled; }
            set
            {
                if (IsPreviewModeEnabled == value) return;
                _isPreviewModeEnabled = value;

                _pollingTimer.IsEnabled = (value || IsRealtimeModeEnabled) && Settings.IsGammaPollingEnabled;

                Debug.WriteLine($"Preview mode {(value ? "enabled" : "disabled")}", GetType().Name);

                UpdateTemperature(true);
                UpdateGamma();

                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Cycle preview time
        /// </summary>
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

        /// <summary>
        /// Whether the cycle preview is currenly running
        /// </summary>
        public bool IsCyclePreviewRunning => _cyclePreviewTimer.IsEnabled;

        public Settings Settings => Settings.Default;

        /// <summary>
        /// Triggered when something changes
        /// </summary>
        public event EventHandler Updated;

        /// <summary>
        /// Triggered when cycle preview ends
        /// </summary>
        public event EventHandler CyclePreviewEnded;

        public TemperatureService(GammaService gammaService)
        {
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
                UpdateTemperature();

                // Ending condition
                if ((CyclePreviewTime - DateTime.Now).TotalHours >= 24)
                {
                    IsPreviewModeEnabled = false;
                    _cyclePreviewTimer.IsEnabled = false;
                    Debug.WriteLine("Ended cycle preview", GetType().Name);
                    CyclePreviewEnded?.Invoke(this, EventArgs.Empty);
                }
            };

            // Helpers
            _temperatureSmoother = new ValueSmoother();

            // Settings
            Settings.PropertyChanged += (sender, args) =>
            {
                UpdateConfiguration();

                if (args.PropertyName.IsEither(nameof(Settings.TemperatureEpsilon), nameof(Settings.MinTemperature),
                    nameof(Settings.MaxTemperature), nameof(Settings.TemperatureSwitchDuration),
                    nameof(Settings.SunriseTime), nameof(Settings.SunsetTime)))
                {
                    UpdateTemperature();
                }
            };

            // System events
            SystemEvents.PowerModeChanged += SystemPowerModeChanged;
            SystemEvents.DisplaySettingsChanged += SystemDisplaySettingsChanged;

            // Init
            _temperature = Settings.DefaultMonitorTemperature;
            UpdateConfiguration();
        }

        private void UpdateConfiguration()
        {
            _temperatureUpdateTimer.Interval = Settings.TemperatureUpdateInterval;

            _pollingTimer.Interval = Settings.GammaPollingInterval;
            _pollingTimer.IsEnabled = (IsRealtimeModeEnabled || IsPreviewModeEnabled) && Settings.IsGammaPollingEnabled;
        }

        private void SystemDisplaySettingsChanged(object sender, EventArgs e)
        {
            UpdateTemperature();
            UpdateGamma();
        }

        private void SystemPowerModeChanged(object sender, PowerModeChangedEventArgs powerModeChangedEventArgs)
        {
            UpdateTemperature();
            UpdateGamma();
        }

        private void UpdateGamma()
        {
            var intens = ColorIntensity.FromTemperature(Temperature);
            _gammaService.SetDisplayGammaLinear(intens);
            Debug.WriteLine($"Gamma updated (to {intens})", GetType().Name);
        }

        private void UpdateTemperature(bool forceInstant = false)
        {
            // 24 hr cycle preview mode
            if (IsPreviewModeEnabled && IsCyclePreviewRunning)
            {
                _temperatureSmoother.Stop(); // stop existing smooth transition
                Temperature = GetTemperature(CyclePreviewTime);
            }
            // Preview mode
            else if (IsPreviewModeEnabled)
            {
                _temperatureSmoother.Stop(); // stop existing smooth transition
                Temperature = _requestedPreviewTemperature;
            }
            // Realtime mode
            else
            {
                ushort newTemp = IsRealtimeModeEnabled
                    ? GetTemperature(DateTime.Now) // on
                    : Settings.DefaultMonitorTemperature; // off
                int diff = Math.Abs(newTemp - Temperature);

                // Smooth transition
                if (!forceInstant &&
                    Settings.IsTemperatureSmoothingEnabled &&
                    diff >= Settings.MinSmoothingDeltaTemperature)
                {
                    _temperatureSmoother.Set(
                        Temperature, newTemp,
                        temp => Temperature = (ushort) temp,
                        Settings.TemperatureSmoothingDuration);

                    Debug.WriteLine($"Started smooth temperature transition (to {newTemp})", GetType().Name);
                }
                // Instant transition
                else
                {
                    _temperatureSmoother.Stop(); // stop existing smooth transition
                    Temperature = newTemp;
                }
            }
        }

        /// <summary>
        /// Request to change temperature to given for preview purposes
        /// </summary>
        public void RequestPreviewTemperature(ushort temp)
        {
            _requestedPreviewTemperature = temp;
        }

        /// <summary>
        /// Starts 24hr cycle preview
        /// </summary>
        public void CyclePreviewStart()
        {
            Debug.WriteLine("Started cycle preview", GetType().Name);

            CyclePreviewTime = DateTime.Now;
            _cyclePreviewTimer.IsEnabled = true;
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

    public partial class TemperatureService
    {
        private static double Ease(double from, double to, double t)
        {
            return from + (to - from)*Math.Sin(t*Math.PI/2);
        }

        private static ushort GetTemperature(TimeSpan time)
        {
            ushort minTemp = Settings.Default.MinTemperature;
            ushort maxTemp = Settings.Default.MaxTemperature;

            var offset = Settings.Default.TemperatureSwitchDuration;
            var halfOffset = TimeSpan.FromHours(offset.TotalHours/2);
            var riseTime = Settings.Default.SunriseTime;
            var setTime = Settings.Default.SunsetTime;
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
                return (ushort) Ease(minTemp, maxTemp, t);
            }

            // Between sunrise and sunset (day time)
            if (time > riseEndTime && time < setStartTime)
                return maxTemp;

            // Incoming sunset (day time -> night time)
            if (time >= setStartTime && time <= setEndTime)
            {
                double t = (time.TotalHours - setStartTime.TotalHours)/offset.TotalHours;
                return (ushort) Ease(maxTemp, minTemp, t);
            }

            // After sunset (night time)
            return minTemp;
        }

        private static ushort GetTemperature(DateTime dt) => GetTemperature(dt.TimeOfDay);
    }
}