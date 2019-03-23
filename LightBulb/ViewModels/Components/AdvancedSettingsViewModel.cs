using LightBulb.Services;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public class AdvancedSettingsViewModel : PropertyChangedBase
    {
        private readonly SettingsService _settingsService;

        // TODO: make it work
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

        // TODO: hotkeys

        public AdvancedSettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;

            // HACK: when settings change - fire property changed event for all properties in this view model
            _settingsService.PropertyChanged += (sender, args) => NotifyOfPropertyChange(null);
        }
    }
}