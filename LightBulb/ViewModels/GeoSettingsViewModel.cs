using System;
using GalaSoft.MvvmLight;

namespace LightBulb.ViewModels
{
    public class GeoSettingsViewModel : ViewModelBase
    {
        public Settings Settings => Settings.Default;

        /// <summary>
        /// Sunrise time in hours
        /// </summary>
        public double SunriseTimeHours
        {
            get { return Settings.SunriseTime.TotalHours; }
            set { Settings.SunriseTime = TimeSpan.FromHours(value); }
        }

        /// <summary>
        /// Sunset time in hours
        /// </summary>
        public double SunsetTimeHours
        {
            get { return Settings.SunsetTime.TotalHours; }
            set { Settings.SunsetTime = TimeSpan.FromHours(value); }
        }

        /// <summary>
        /// Whether geoinfo is set
        /// </summary>
        public bool GeoinfoNotNull => Settings.GeoInfo != null;

        public GeoSettingsViewModel()
        {
            // Settings
            Settings.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Settings.SunriseTime))
                    RaisePropertyChanged(() => SunriseTimeHours);
                else if (args.PropertyName == nameof(Settings.SunsetTime))
                    RaisePropertyChanged(() => SunsetTimeHours);
                else if (args.PropertyName == nameof(Settings.GeoInfo))
                    RaisePropertyChanged(() => GeoinfoNotNull);
            };
        }
    }
}