using LightBulb.Messages;
using LightBulb.Models;
using LightBulb.Services;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public class HotKeySettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly SettingsService _settingsService;
        private readonly SystemService _systemService;

        public HotKey ToggleHotKey
        {
            get => _settingsService.ToggleHotKey;
            set => _settingsService.ToggleHotKey = value;
        }

        public HotKeySettingsTabViewModel(IEventAggregator eventAggregator, SettingsService settingsService,
            SystemService systemService)
            : base(3, "Hotkeys")
        {
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            _systemService = systemService;

            // Re-register hotkeys when they change
            this.Bind(o => o.ToggleHotKey, (sender, args) => RegisterHotKeys());
        }

        public void RegisterHotKeys()
        {
            _systemService.UnregisterAllHotKeys();

            if (ToggleHotKey != HotKey.None)
                _systemService.RegisterHotKey(ToggleHotKey,
                    () => _eventAggregator.Publish(new ToggleIsEnabledMessage()));
        }
    }
}