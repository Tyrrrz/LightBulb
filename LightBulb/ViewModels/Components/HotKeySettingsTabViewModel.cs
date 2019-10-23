using LightBulb.Messages;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.ViewModels.Framework;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public class HotKeySettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly SettingsService _settingsService;
        private readonly SystemService _systemService;

        public HotKeyViewModel ToggleHotKey { get; }

        public HotKeySettingsTabViewModel(IEventAggregator eventAggregator, IViewModelFactory viewModelFactory, SettingsService settingsService,
            SystemService systemService)
            : base(3, "Hotkeys")
        {
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            _systemService = systemService;

            // Initialize view models
            ToggleHotKey = viewModelFactory.CreateHotKeyViewModel();

            // Update hotkeys when they change in settings
            _settingsService.Bind(o => o.ToggleHotKey, (sender, args) => ToggleHotKey.Model = _settingsService.ToggleHotKey);

            // Re-register hotkeys when they change
            ToggleHotKey.Bind(o => o.Model, (sender, args) =>
            {
                _settingsService.ToggleHotKey = ToggleHotKey;
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
        }
    }
}