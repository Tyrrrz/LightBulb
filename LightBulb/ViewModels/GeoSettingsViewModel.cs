using System;
using GalaSoft.MvvmLight;
using LightBulb.Services.Interfaces;

namespace LightBulb.ViewModels
{
    public class GeoSettingsViewModel : ViewModelBase
    {
        public ISettingsService SettingsService { get; }

        /// <summary>
        /// Sunrise time in hours
        /// </summary>
        public double SunriseTimeHours
        {
            get { return SettingsService.SunriseTime.TotalHours; }
            set { SettingsService.SunriseTime = TimeSpan.FromHours(value); }
        }

        /// <summary>
        /// Sunset time in hours
        /// </summary>
        public double SunsetTimeHours
        {
            get { return SettingsService.SunsetTime.TotalHours; }
            set { SettingsService.SunsetTime = TimeSpan.FromHours(value); }
        }

        /// <summary>
        /// Whether geoinfo is set
        /// </summary>
        public bool GeoinfoNotNull => SettingsService.GeoInfo != null;

        public GeoSettingsViewModel(ISettingsService settingsService)
        {
            SettingsService = settingsService;

            // Settings
            SettingsService.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SettingsService.SunriseTime))
                    RaisePropertyChanged(() => SunriseTimeHours);
                else if (args.PropertyName == nameof(SettingsService.SunsetTime))
                    RaisePropertyChanged(() => SunsetTimeHours);
                else if (args.PropertyName == nameof(SettingsService.GeoInfo))
                    RaisePropertyChanged(() => GeoinfoNotNull);
            };
        }
    }
}