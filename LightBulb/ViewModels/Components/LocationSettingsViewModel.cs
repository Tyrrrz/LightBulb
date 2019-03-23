﻿using System;
using LightBulb.Models;
using LightBulb.Services;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public class LocationSettingsViewModel : PropertyChangedBase
    {
        private readonly SettingsService _settingsService;

        public TimeSpan SunriseTime
        {
            get => _settingsService.SunriseTime;
            set => _settingsService.SunriseTime = value;
        }

        public TimeSpan SunsetTime
        {
            get => _settingsService.SunsetTime;
            set => _settingsService.SunsetTime = value;
        }

        public GeoLocation? Location
        {
            get => _settingsService.Location;
            set => _settingsService.Location = value;
        }

        public bool IsInternetSyncEnabled
        {
            get => _settingsService.IsInternetSyncEnabled;
            set => _settingsService.IsInternetSyncEnabled = value;
        }

        public LocationSettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;

            // HACK: when settings change - fire property changed event for all properties in this view model
            _settingsService.PropertyChanged += (sender, args) => NotifyOfPropertyChange(null);
        }
    }
}