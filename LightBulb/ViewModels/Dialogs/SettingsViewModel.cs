using System.Collections.Generic;
using System.Linq;
using LightBulb.Services;
using LightBulb.ViewModels.Components.Settings;
using LightBulb.ViewModels.Framework;

namespace LightBulb.ViewModels.Dialogs
{
    public class SettingsViewModel : DialogScreen
    {
        private readonly SettingsService _settingsService;

        public IReadOnlyList<ISettingsTabViewModel> Tabs { get; }

        public ISettingsTabViewModel? ActiveTab { get; private set; }

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

            ActiveTab = settingsTab;
            settingsTab.IsActive = true;
        }

        // This should just be an overload, but Stylet gets confused when there are two methods with same name
        public void ActivateTabByType<T>() where T : ISettingsTabViewModel
        {
            var tab = Tabs.OfType<T>().FirstOrDefault();
            if (tab != null)
                ActivateTab(tab);
        }

        public void Reset() => _settingsService.Reset();

        public void Save()
        {
            _settingsService.Save();
            Close(true);
        }

        public void Cancel()
        {
            _settingsService.Load();
            Close(false);
        }
    }
}