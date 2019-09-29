using System.Collections.Generic;
using System.Linq;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Framework;

namespace LightBulb.ViewModels.Dialogs
{
    public class SettingsViewModel : DialogScreen
    {
        public IReadOnlyList<SettingsTabViewModel> Tabs { get; }

        public SettingsTabViewModel ActiveTab { get; private set; }

        public SettingsViewModel(IViewModelFactory viewModelFactory)
        {
            // Create view models for tabs
            Tabs = new SettingsTabViewModel[]
            {
                viewModelFactory.CreateGeneralSettingsTabViewModel(),
                viewModelFactory.CreateLocationSettingsTabViewModel(),
                viewModelFactory.CreateAdvancedSettingsTabViewModel()
            };

            // Pre-select first tab
            var firstTab = Tabs.FirstOrDefault();
            if (firstTab != null)
                ActivateTab(firstTab);
        }

        public void ActivateTab(SettingsTabViewModel settingsTab)
        {
            // Deactivate previously selected tab
            if (ActiveTab != null)
                ActiveTab.IsActive = false;

            // Set new tab
            ActiveTab = settingsTab;
            ActiveTab.IsActive = true;
        }
    }
}