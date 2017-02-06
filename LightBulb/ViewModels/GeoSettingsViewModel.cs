using System;
using GalaSoft.MvvmLight;
using LightBulb.Services.Interfaces;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels
{
    public class GeoSettingsViewModel : ViewModelBase
    {
        private bool _geoInfoNotNull;
        private string _geoInfoCountryFlagUrl;

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
        public bool GeoInfoNotNull
        {
            get { return _geoInfoNotNull; }
            private set { Set(ref _geoInfoNotNull, value); }
        }

        /// <summary>
        /// Flag image url for current country
        /// </summary>
        public string GeoInfoCountryFlagUrl
        {
            get { return _geoInfoCountryFlagUrl; }
            private set { Set(ref _geoInfoCountryFlagUrl, value); }
        }

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
                {
                    GeoInfoNotNull = SettingsService.GeoInfo != null;
                    GeoInfoCountryFlagUrl = SettingsService.GeoInfo == null ||
                                            SettingsService.GeoInfo.CountryCode.IsBlank()
                        ? "https://cdn2.f-cdn.com/img/flags/png/unknown.png"
                        : $"https://cdn2.f-cdn.com/img/flags/png/{SettingsService.GeoInfo.CountryCode.ToLowerInvariant()}.png";
                }
            };
        }
    }
}