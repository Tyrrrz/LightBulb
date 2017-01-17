using System;
using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.Services.Helpers;
using NegativeLayer.WPFExtensions;

namespace LightBulb.ViewModels
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private readonly TemperatureService _temperatureService;
        private readonly WindowService _windowService;
        // ReSharper disable once NotAccessedField.Local
        private readonly GeoSyncService _geoSyncService;

        private readonly Timer _disableTemporarilyTimer;

        private bool _isEnabled;
        private bool _isBlocked;
        private string _statusText;
        private CycleState _cycleState;

        public Settings Settings => Settings.Default;

        /// <summary>
        /// Enables or disables the program
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (!Set(ref _isEnabled, value)) return;
                if (value) _disableTemporarilyTimer.IsEnabled = false;

                _temperatureService.IsRealtimeModeEnabled = value && !IsBlocked;

                UpdateStatusText();
                UpdateCycleState();
            }
        }

        /// <summary>
        /// Whether gamma control is blocked by something 
        /// </summary>
        public bool IsBlocked
        {
            get { return _isBlocked; }
            private set
            {
                if (!Set(ref _isBlocked, value)) return;

                _temperatureService.IsRealtimeModeEnabled = !value && IsEnabled;

                UpdateStatusText();
                UpdateCycleState();
            }
        }

        /// <summary>
        /// Enables or disables the preview mode
        /// </summary>
        public bool IsInPreviewMode
        {
            get { return _temperatureService.IsPreviewModeEnabled; }
            set { _temperatureService.IsPreviewModeEnabled = value; }
        }

        /// <summary>
        /// Temperature for preview mode
        /// </summary>
        public ushort PreviewTemperature
        {
            get { return _temperatureService.PreviewTemperature; }
            set { _temperatureService.PreviewTemperature = value; }
        }

        /// <summary>
        /// Current status text
        /// </summary>
        public string StatusText
        {
            get { return _statusText; }
            private set { Set(ref _statusText, value); }
        }

        /// <summary>
        /// State of the current cycle
        /// </summary>
        public CycleState CycleState
        {
            get { return _cycleState; }
            private set { Set(ref _cycleState, value); }
        }

        // Commands
        public RelayCommand ShowMainWindowCommand { get; }
        public RelayCommand ExitApplicationCommand { get; }
        public RelayCommand AboutCommand { get; }
        public RelayCommand ToggleEnabledCommand { get; }
        public RelayCommand<double> DisableTemporarilyCommand { get; }
        public RelayCommand StartCyclePreviewCommand { get; }

        public MainViewModel(
            TemperatureService temperatureService,
            WindowService windowService,
            GeoSyncService geoSyncService)
        {
            // Services
            _temperatureService = temperatureService;
            _windowService = windowService;
            _geoSyncService = geoSyncService;

            _temperatureService.Updated += (sender, args) =>
            {
                UpdateStatusText();
                UpdateCycleState();
            };
            _temperatureService.CyclePreviewEnded += (sender, args) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => StartCyclePreviewCommand.RaiseCanExecuteChanged());
            };
            _windowService.FullScreenStateChanged += (sender, args) =>
            {
                UpdateBlock();
            };

            // Timers
            _disableTemporarilyTimer = new Timer();
            _disableTemporarilyTimer.Tick += (sender, args) =>
            {
                IsEnabled = true;
            };

            // Commands
            ShowMainWindowCommand = new RelayCommand(() =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Application.Current.MainWindow.Show();
                    Application.Current.MainWindow.Activate();
                    Application.Current.MainWindow.Focus();
                });
            });
            ExitApplicationCommand = new RelayCommand(() =>
            {
                Application.Current.ShutdownSafe();
            });
            AboutCommand = new RelayCommand(() =>
            {
                Process.Start("https://github.com/Tyrrrz/LightBulb");
            });
            ToggleEnabledCommand = new RelayCommand(() =>
            {
                IsEnabled = !IsEnabled;
            });
            DisableTemporarilyCommand = new RelayCommand<double>(ms =>
            {
                _disableTemporarilyTimer.IsEnabled = false;
                _disableTemporarilyTimer.Interval = TimeSpan.FromMilliseconds(ms);
                _disableTemporarilyTimer.IsEnabled = true;
                IsEnabled = false;
            });
            StartCyclePreviewCommand = new RelayCommand(() =>
                {
                    _temperatureService.CyclePreviewStart();
                },
                () => !_temperatureService.IsCyclePreviewRunning);

            // Init
            IsEnabled = true;
        }

        private void UpdateBlock()
        {
            IsBlocked = Settings.IsFullscreenBlocking && _windowService.IsForegroundFullScreen;

            Debug.WriteLine($"Updated block status (to {IsBlocked})", GetType().Name);
        }

        private void UpdateStatusText()
        {
            // Preview mode (not 24hr cycle preview)
            if (IsInPreviewMode && !_temperatureService.IsCyclePreviewRunning)
            {
                StatusText = $"Temp: {_temperatureService.PreviewTemperature}K   (preview)";
            }
            // Preview mode (24 hr cycle preview)
            else if (IsInPreviewMode && _temperatureService.IsCyclePreviewRunning)
            {
                StatusText =
                    $"Temp: {_temperatureService.PreviewTemperature}K   Time: {_temperatureService.CyclePreviewTime:t}   (preview)";
            }
            // Not enabled
            else if (!IsEnabled)
            {
                StatusText = "LightBulb is off";
            }
            // Blocked
            else if (IsBlocked)
            {
                StatusText = "LightBulb is blocked";
            }
            // Realtime mode
            else
            {
                StatusText =
                    $"Temp: {_temperatureService.RealtimeTemperature}K   Time: {_temperatureService.Time:t}";
            }
        }

        private void UpdateCycleState()
        {
            if (!IsEnabled || IsBlocked)
            {
                CycleState = CycleState.Disabled;
            }
            else
            {
                ushort temp = _temperatureService.IsPreviewModeEnabled
                    ? _temperatureService.PreviewTemperature
                    : _temperatureService.RealtimeTemperature;
                if (temp >= Settings.MaxTemperature)
                {
                    CycleState = CycleState.Day;
                }
                else if (temp <= Settings.MinTemperature)
                {
                    CycleState = CycleState.Night;
                }
                else
                {
                    CycleState = CycleState.Transition;
                }
            }
        }

        public void Dispose()
        {
            _disableTemporarilyTimer.Dispose();
        }
    }
}