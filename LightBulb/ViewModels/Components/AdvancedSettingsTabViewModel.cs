using LightBulb.Services;

namespace LightBulb.ViewModels.Components
{
    public class AdvancedSettingsTabViewModel : SettingsTabViewModelBase
    {
        public bool IsAutoStartEnabled
        {
            get => SettingsService.IsAutoStartEnabled;
            set => SettingsService.IsAutoStartEnabled = value;
        }

        public bool IsDefaultToDayConfigurationEnabled
        {
            get => SettingsService.IsDefaultToDayConfigurationEnabled;
            set => SettingsService.IsDefaultToDayConfigurationEnabled = value;
        }

        public bool IsConfigurationSmoothingEnabled
        {
            get => SettingsService.IsConfigurationSmoothingEnabled;
            set => SettingsService.IsConfigurationSmoothingEnabled = value;
        }

        public bool IsPauseWhenFullScreenEnabled
        {
            get => SettingsService.IsPauseWhenFullScreenEnabled;
            set => SettingsService.IsPauseWhenFullScreenEnabled = value;
        }

        public AdvancedSettingsTabViewModel(SettingsService settingsService)
            : base(settingsService, 2, "Advanced")
        {
        }
    }
}