using System.Collections.Generic;
using System.Linq;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Framework;

namespace LightBulb.ViewModels.Dialogs
{
    public class SettingsViewModel : DialogScreen
    {
        public IReadOnlyList<ISettingsTabViewModel> Tabs { get; }

        public ISettingsTabViewModel ActiveTab { get; private set; }

        public SettingsViewModel(IEnumerable<ISettingsTabViewModel> tabs)
        {
            Tabs = tabs.OrderBy(t => t.Order).ToArray();

            // Pre-select first tab
            var firstTab = Tabs.FirstOrDefault();
            if (firstTab != null)
                ActivateTab(firstTab);
        }

        public void ActivateTab(ISettingsTabViewModel settingsTab)
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