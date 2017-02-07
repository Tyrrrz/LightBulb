using System;
using GalaSoft.MvvmLight;
using LightBulb.Services.Interfaces;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels
{
    public sealed class GeoSettingsViewModel : ViewModelBase
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
        public bool GeoInfoNotNull => SettingsService.GeoInfo != null;

        /// <summary>
        /// Flag image url for current country
        /// </summary>
        public string GeoInfoCountryFlagUrl => GeoInfoNotNull && SettingsService.GeoInfo.CountryCode.IsNotBlank()
            ? $"https://cdn2.f-cdn.com/img/flags/png/{SettingsService.GeoInfo.CountryCode.ToLowerInvariant()}.png"
            : "https://cdn2.f-cdn.com/img/flags/png/unknown.png";

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
                    RaisePropertyChanged(() => GeoInfoNotNull);
                    RaisePropertyChanged(() => GeoInfoCountryFlagUrl);
                }
            };
        }
    }
}