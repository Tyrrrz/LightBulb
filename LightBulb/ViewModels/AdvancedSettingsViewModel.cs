using GalaSoft.MvvmLight;
using LightBulb.Services.Interfaces;
using LightBulb.ViewModels.Interfaces;

namespace LightBulb.ViewModels
{
    public class AdvancedSettingsViewModel : ViewModelBase, IAdvancedSettingsViewModel
    {
        /// <inheritdoc />
        public ISettingsService SettingsService { get; }

        public AdvancedSettingsViewModel(ISettingsService settingsService)
        {
            SettingsService = settingsService;
        }
    }
}