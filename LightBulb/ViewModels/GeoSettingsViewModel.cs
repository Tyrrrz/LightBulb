using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using LightBulb.Services;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels
{
    public class GeoSettingsViewModel : ViewModelBase, IGeoSettingsViewModel, IDisposable
    {
        /// <inheritdoc />
        public ISettingsService SettingsService { get; }

        /// <inheritdoc />
        public bool IsGeoInfoSet => SettingsService.GeoInfo != null;

        /// <inheritdoc />
        public string GeoInfoCountryFlagUrl => IsGeoInfoSet && SettingsService.GeoInfo.CountryCode.IsNotBlank()
            ? $"https://cdn2.f-cdn.com/img/flags/png/{SettingsService.GeoInfo.CountryCode.ToLowerInvariant()}.png"
            : "https://cdn2.f-cdn.com/img/flags/png/unknown.png";

        public GeoSettingsViewModel(ISettingsService settingsService)
        {
            SettingsService = settingsService;

            // Settings
            SettingsService.PropertyChanged += SettingsServicePropertyChanged;
        }

        ~GeoSettingsViewModel()
        {
            Dispose(false);
        }

        private void SettingsServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(ISettingsService.GeoInfo))
            {
                RaisePropertyChanged(() => IsGeoInfoSet);
                RaisePropertyChanged(() => GeoInfoCountryFlagUrl);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                SettingsService.PropertyChanged -= SettingsServicePropertyChanged;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}