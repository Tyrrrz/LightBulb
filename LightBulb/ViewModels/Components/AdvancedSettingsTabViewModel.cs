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

        public bool IsAutoUpdateEnabled
        {
            get => SettingsService.IsAutoUpdateEnabled;
            set => SettingsService.IsAutoUpdateEnabled = value;
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

        public bool IsGammaPollingEnabled
        {
            get => SettingsService.IsGammaPollingEnabled;
            set => SettingsService.IsGammaPollingEnabled = value;
        }

        public AdvancedSettingsTabViewModel(SettingsService settingsService)
            : base(settingsService, 2, "Advanced")
        {
        }
    }
}