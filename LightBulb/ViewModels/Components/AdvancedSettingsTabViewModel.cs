using LightBulb.Services;

namespace LightBulb.ViewModels.Components
{
    public class AdvancedSettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly SystemService _systemService;

        // HACK: this doesn't go through SettingsService and is not affected by Save/Reset/etc
        public bool IsAutoStartEnabled
        {
            get => _systemService.IsAutoStartEnabled();
            set
            {
                if (value)
                    _systemService.EnableAutoStart();
                else
                    _systemService.DisableAutoStart();
            }
        }

        public bool IsDefaultToDayConfigurationEnabled
        {
            get => SettingsService.IsDefaultToDayConfigurationEnabled;
            set => SettingsService.IsDefaultToDayConfigurationEnabled = value;
        }

        public bool IsGammaSmoothingEnabled
        {
            get => SettingsService.IsGammaSmoothingEnabled;
            set => SettingsService.IsGammaSmoothingEnabled = value;
        }

        public bool IsPauseWhenFullScreenEnabled
        {
            get => SettingsService.IsPauseWhenFullScreenEnabled;
            set => SettingsService.IsPauseWhenFullScreenEnabled = value;
        }

        public AdvancedSettingsTabViewModel(SettingsService settingsService, SystemService systemService)
            : base(settingsService, 2, "Advanced")
        {
            _systemService = systemService;
        }
    }
}