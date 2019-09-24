using LightBulb.Messages;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.ViewModels.Framework;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public class AdvancedSettingsViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly SettingsService _settingsService;
        private readonly SystemService _systemService;

        public bool IsGammaPollingEnabled
        {
            get => _settingsService.IsGammaPollingEnabled;
            set => _settingsService.IsGammaPollingEnabled = value;
        }

        public bool IsGammaSmoothingEnabled
        {
            get => _settingsService.IsGammaSmoothingEnabled;
            set => _settingsService.IsGammaSmoothingEnabled = value;
        }

        public bool IsPauseWhenFullScreenEnabled
        {
            get => _settingsService.IsPauseWhenFullScreenEnabled;
            set => _settingsService.IsPauseWhenFullScreenEnabled = value;
        }

        public HotKeyViewModel ToggleHotKey { get; }

        public HotKeyViewModel ToggleGammaPollingHotKey { get; }

        public AdvancedSettingsViewModel(IEventAggregator eventAggregator, IViewModelFactory viewModelFactory,
            SettingsService settingsService, SystemService systemService)
        {
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            _systemService = systemService;

            // Initialize view models
            ToggleHotKey = viewModelFactory.CreateHotKeyViewModel();
            ToggleGammaPollingHotKey = viewModelFactory.CreateHotKeyViewModel();

            // HACK: when settings change - fire property changed event for all properties in this view model
            _settingsService.PropertyChanged += (sender, args) => NotifyOfPropertyChange(null);

            // Update hotkeys when they change in settings
            _settingsService.Bind(o => o.ToggleHotKey, (sender, args) => ToggleHotKey.Model = _settingsService.ToggleHotKey);
            _settingsService.Bind(o => o.ToggleGammaPollingHotKey,
                (sender, args) => ToggleGammaPollingHotKey.Model = _settingsService.ToggleGammaPollingHotKey);

            // Re-register hotkeys when they change
            ToggleHotKey.Bind(o => o.Model, (sender, args) =>
            {
                _settingsService.ToggleHotKey = ToggleHotKey;
                RegisterHotKeys();
            });

            ToggleGammaPollingHotKey.Bind(o => o.Model, (sender, args) =>
            {
                _settingsService.ToggleGammaPollingHotKey = ToggleGammaPollingHotKey;
                RegisterHotKeys();
            });
        }

        public void RegisterHotKeys()
        {
            _systemService.UnregisterAllHotKeys();

            // TODO: rework later
            if (ToggleHotKey != HotKey.None)
                _systemService.RegisterHotKey(ToggleHotKey,
                    () => _eventAggregator.Publish(new ToggleIsEnabledMessage()));

            if (ToggleGammaPollingHotKey != HotKey.None)
                _systemService.RegisterHotKey(ToggleGammaPollingHotKey,
                    () => _settingsService.IsGammaPollingEnabled = !_settingsService.IsGammaPollingEnabled);
        }
    }
}