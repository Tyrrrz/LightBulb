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

        public HotKey IncreaseTemperatureOffsetHotKey
        {
            get => SettingsService.IncreaseTemperatureOffsetHotKey;
            set => SettingsService.IncreaseTemperatureOffsetHotKey = value;
        }

        public HotKey DecreaseTemperatureOffsetHotKey
        {
            get => SettingsService.DecreaseTemperatureOffsetHotKey;
            set => SettingsService.DecreaseTemperatureOffsetHotKey = value;
        }

        public HotKey IncreaseBrightnessOffsetHotKey
        {
            get => SettingsService.IncreaseBrightnessOffsetHotKey;
            set => SettingsService.IncreaseBrightnessOffsetHotKey = value;
        }

        public HotKey DecreaseBrightnessOffsetHotKey
        {
            get => SettingsService.DecreaseBrightnessOffsetHotKey;
            set => SettingsService.DecreaseBrightnessOffsetHotKey = value;
        }

        public HotKey ResetOffsetHotKey
        {
            get => SettingsService.ResetOffsetHotKey;
            set => SettingsService.ResetOffsetHotKey = value;
        }

        public HotKeySettingsTabViewModel(SettingsService settingsService)
            : base(settingsService, 4, "Hotkeys")
        {
        }
    }
}