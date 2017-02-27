using GalaSoft.MvvmLight;
using LightBulb.Services.Interfaces;
using LightBulb.ViewModels.Interfaces;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels
{
    public class GeoSettingsViewModel : ViewModelBase, IGeoSettingsViewModel
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
            SettingsService.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SettingsService.GeoInfo))
                {
                    RaisePropertyChanged(() => IsGeoInfoSet);
                    RaisePropertyChanged(() => GeoInfoCountryFlagUrl);
                }
            };
        }
    }
}