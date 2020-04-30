using LightBulb.Models;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components.Settings
{
    public class HotKeySettingsTabViewModel : SettingsTabViewModelBase
    {
        public HotKey ToggleHotKey
        {
            get => SettingsService.ToggleHotKey;
            set => SettingsService.ToggleHotKey = value;
        }

        public HotKeySettingsTabViewModel(SettingsService settingsService)
            : base(settingsService, 4, "Hotkeys")
        {
        }
    }
}