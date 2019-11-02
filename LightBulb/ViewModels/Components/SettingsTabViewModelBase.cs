using LightBulb.Services;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public abstract class SettingsTabViewModelBase : PropertyChangedBase, ISettingsTabViewModel
    {
        protected SettingsService SettingsService { get; }

        public int Order { get; }

        public string DisplayName { get; }

        public bool IsActive { get; set; }

        protected SettingsTabViewModelBase(SettingsService settingsService, int order, string displayName)
        {
            SettingsService = settingsService;
            Order = order;
            DisplayName = displayName;

            // Route property changes from settings service (e.g. when settings are reset)
            SettingsService.PropertyChanged += (sender, args) => Refresh();
        }
    }
}