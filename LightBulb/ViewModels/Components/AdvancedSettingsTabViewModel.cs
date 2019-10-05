﻿using LightBulb.Internal;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components
{
    public class AdvancedSettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly SettingsService _settingsService;
        private readonly SystemService _systemService;

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

        public AdvancedSettingsTabViewModel(SettingsService settingsService, SystemService systemService)
            : base(2, "Advanced")
        {
            _settingsService = settingsService;
            _systemService = systemService;

            // HACK: when settings change - fire property changed event for all properties in this view model
            _settingsService.Bind((sender, args) => NotifyOfPropertyChange(null));
        }
    }
}