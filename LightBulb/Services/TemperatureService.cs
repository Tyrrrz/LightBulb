using System;
using System.Diagnostics;
using LightBulb.Models;
using LightBulb.Services.Helpers;
using NegativeLayer.Extensions;

namespace LightBulb.Services
{
    public partial class TemperatureService : IDisposable
    {
        private readonly GammaService _gammaService;

        private readonly SyncedTimer _updateTimer;
        private readonly Timer _cyclePreviewTimer;
        private readonly Timer _pollingTimer;
        private readonly ValueSmoother _temperatureSmoother;

        private bool _isRealtimeModeEnabled;
        private bool _isPreviewModeEnabled;
        private DateTime _time;
        private DateTime _cyclePreviewTime;
        private ushort _realtimeTemperature;
        private ushort _previewTemperature;

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

                _updateTimer.IsEnabled = value;
                _pollingTimer.IsEnabled = (value || IsPreviewModeEnabled) && Settings.IsGammaPollingEnabled;

                UpdateRealtimeTemperature();
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

                // Stop ongoing 24hr cycle preview
                if (!value && _cyclePreviewTimer.IsEnabled)
                {
                    _cyclePreviewTimer.IsEnabled = false;
                    CyclePreviewEnded?.Invoke(this, EventArgs.Empty);
                }

                _pollingTimer.IsEnabled = (value || IsRealtimeModeEnabled) && Settings.IsGammaPollingEnabled;

                UpdateGamma();

                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Last temperature update time
        /// </summary>
        public DateTime Time
        {
            get { return _time; }
            private set
            {
                if (Time == value) return;
                _time = value;

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

        /// <summary>
        /// Current color temperature
        /// </summary>
        public ushort RealtimeTemperature
        {
            get { return _realtimeTemperature; }
            private set
            {
                if (value < Settings.MinTemperature) value = Settings.MinTemperature;
                if (value > Settings.MaxTemperature) value = Settings.MaxTemperature;
                if (RealtimeTemperature == value) return;
                int diff = Math.Abs(RealtimeTemperature - value);
                if (diff <= Settings.TemperatureEpsilon &&
                    !value.IsEither(Settings.MaxTemperature, Settings.MinTemperature)) return;
                _realtimeTemperature = value;

                Debug.WriteLine($"Updated temperature (to {value})", GetType().Name);

                UpdateGamma();

                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Current preview temperature
        /// </summary>
        public ushort PreviewTemperature
        {
            get { return _previewTemperature; }
            set
            {
                if (value < Settings.MinTemperature) value = Settings.MinTemperature;
                if (value > Settings.MaxTemperature) value = Settings.MaxTemperature;
                if (PreviewTemperature == value) return;
                int diff = Math.Abs(PreviewTemperature - value);
                if (diff <= Settings.TemperatureEpsilon &&
                    !value.IsEither(Settings.MaxTemperature, Settings.MinTemperature)) return;
                _previewTemperature = value;

                Debug.WriteLine($"Updated preview temperature (to {value})", GetType().Name);

                UpdateGamma();

                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

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

            _updateTimer = new SyncedTimer();
            _updateTimer.Tick += (sender, args) =>
            {
                UpdateRealtimeTemperature();
            };

            _cyclePreviewTimer = new Timer(TimeSpan.FromMilliseconds(15));
            _cyclePreviewTimer.Tick += (sender, args) =>
            {
                CyclePreviewUpdateTemperature();

                // Ending condition
                if ((CyclePreviewTime - Time).TotalHours >= 24)
                {
                    IsPreviewModeEnabled = false;
                    _cyclePreviewTimer.IsEnabled = false;
                    Debug.WriteLine("Ended cycle preview", GetType().Name);
                    CyclePreviewEnded?.Invoke(this, EventArgs.Empty);
                }
            };

            _pollingTimer = new Timer();
            _pollingTimer.Tick += (sender, args) => UpdateGamma();

            _temperatureSmoother = new ValueSmoother();

            RealtimeTemperature = Settings.DefaultMonitorTemperature;

            Settings.PropertyChanged += (sender, args) =>
            {
                UpdateConfiguration();

                if (args.PropertyName.IsEither(nameof(Settings.TemperatureEpsilon), nameof(Settings.MinTemperature),
                    nameof(Settings.MaxTemperature), nameof(Settings.TemperatureSwitchDuration),
                    nameof(Settings.SunriseTime), nameof(Settings.SunsetTime)))
                {
                    UpdateRealtimeTemperature();
                }
            };

            UpdateConfiguration();
        }

        private void UpdateConfiguration()
        {
            _updateTimer.Interval = Settings.TemperatureUpdateInterval;

            _pollingTimer.Interval = Settings.GammaPollingInterval;
            _pollingTimer.IsEnabled = (IsRealtimeModeEnabled || IsPreviewModeEnabled) && Settings.IsGammaPollingEnabled;
        }

        private void UpdateGamma()
        {
            ushort temp = IsPreviewModeEnabled ? PreviewTemperature : RealtimeTemperature;
            var intens = ColorIntensity.FromTemperature(temp);
            _gammaService.SetDisplayGammaLinear(intens);

            Debug.WriteLine($"Gamma updated (to {intens})", GetType().Name);
        }

        private void UpdateRealtimeTemperature()
        {
            Time = DateTime.Now;
            ushort newTemp = IsRealtimeModeEnabled ? GetTemperature(Time) : Settings.DefaultMonitorTemperature;
            int diff = Math.Abs(newTemp - RealtimeTemperature);

            // Smooth transition
            if (Settings.IsTemperatureSmoothingEnabled && diff >= Settings.MinSmoothingDeltaTemperature)
            {
                _temperatureSmoother.Set(RealtimeTemperature, newTemp, temp => RealtimeTemperature = (ushort) temp,
                    Settings.TemperatureSmoothingDuration);

                Debug.WriteLine($"Started smooth temperature transition (to {newTemp})", GetType().Name);
            }
            // Instant transition
            else
            {
                _temperatureSmoother.Stop();
                RealtimeTemperature = newTemp;
            }
        }

        private void CyclePreviewUpdateTemperature()
        {
            CyclePreviewTime = CyclePreviewTime.Add(TimeSpan.FromHours(0.05));
            PreviewTemperature = GetTemperature(CyclePreviewTime);
        }

        public void CyclePreviewStart()
        {
            Debug.WriteLine("Started cycle preview", GetType().Name);

            CyclePreviewTime = Time;
            PreviewTemperature = GetTemperature(CyclePreviewTime);
            IsPreviewModeEnabled = true;
            _cyclePreviewTimer.IsEnabled = true;
        }

        public virtual void Dispose()
        {
            _updateTimer.Dispose();
            _cyclePreviewTimer.Dispose();
            _pollingTimer.Dispose();
            _temperatureSmoother.Dispose();
        }
    }

    public partial class TemperatureService
    {
        private static double Ease(double from, double to, double t)
        {
            t = t.Clamp(0, 1);
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