using LightBulb.Models;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components
{
    public class HotKeySettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly SettingsService _settingsService;

        public HotKey ToggleHotKey
        {
            get => _settingsService.ToggleHotKey;
            set => _settingsService.ToggleHotKey = value;
        }

        public HotKeySettingsTabViewModel(SettingsService settingsService)
            : base(3, "Hotkeys")
        {
            _settingsService = settingsService;
        }
    }
}