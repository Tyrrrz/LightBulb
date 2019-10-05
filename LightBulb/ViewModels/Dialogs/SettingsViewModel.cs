using System.Collections.Generic;
using System.Linq;
using LightBulb.Services;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Framework;

namespace LightBulb.ViewModels.Dialogs
{
    public class SettingsViewModel : DialogScreen
    {
        private readonly SettingsService _settingsService;

        public IReadOnlyList<ISettingsTabViewModel> Tabs { get; }

        public ISettingsTabViewModel ActiveTab { get; private set; }

        public SettingsViewModel(SettingsService settingsService, IEnumerable<ISettingsTabViewModel> tabs)
        {
            _settingsService = settingsService;

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

        public void ResetSettings() => _settingsService.Reset();
    }
}