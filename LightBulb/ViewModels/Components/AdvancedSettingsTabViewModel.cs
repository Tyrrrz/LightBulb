using LightBulb.Services;

namespace LightBulb.ViewModels.Components
{
    public class AdvancedSettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly RegistryService _registryService;

        // HACK: this doesn't go through SettingsService and is not affected by Save/Reset/etc
        public bool IsAutoStartEnabled
        {
            get => _registryService.IsAutoStartEnabled();
            set
            {
                if (value)
                    _registryService.EnableAutoStart();
                else
                    _registryService.DisableAutoStart();
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

        public AdvancedSettingsTabViewModel(SettingsService settingsService, RegistryService registryService)
            : base(settingsService, 2, "Advanced")
        {
            _registryService = registryService;
        }
    }
}