using System.Collections.Generic;
using System.Linq;
using LightBulb.Messages;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Framework;
using Stylet;

namespace LightBulb.ViewModels.Dialogs
{
    public class SettingsViewModel : DialogScreen
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly SettingsService _settingsService;
        private readonly SystemService _systemService;

        public IReadOnlyList<ISettingsTabViewModel> Tabs { get; }

        public ISettingsTabViewModel ActiveTab { get; private set; }

        public SettingsViewModel(IEventAggregator eventAggregator, SettingsService settingsService, SystemService systemService,
            IEnumerable<ISettingsTabViewModel> tabs)
        {
            _settingsService = settingsService;
            _eventAggregator = eventAggregator;
            _systemService = systemService;

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

        public void Reset() => _settingsService.Reset();

        public void Save()
        {
            _settingsService.Save();
            RegisterHotKeys();
            Close(true);
        }

        public void Cancel()
        {
            _settingsService.Load();
            Close(false);
        }

        public void RegisterHotKeys()
        {
            _systemService.UnregisterAllHotKeys();

            if (_settingsService.ToggleHotKey != HotKey.None)
            {
                _systemService.RegisterHotKey(_settingsService.ToggleHotKey, () => _eventAggregator.Publish(new ToggleIsEnabledMessage()));
            }
        }
    }
}