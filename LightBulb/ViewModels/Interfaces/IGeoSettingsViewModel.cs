using LightBulb.Services.Interfaces;

namespace LightBulb.ViewModels.Interfaces
{
    public interface IGeoSettingsViewModel
    {
        /// <summary>
        /// Settings interface
        /// </summary>
        ISettingsService SettingsService { get; }

        /// <summary>
        /// Whether geoinfo is set
        /// </summary>
        bool IsGeoInfoSet { get; }

        /// <summary>
        /// Flag image url for current country
        /// </summary>
        string GeoInfoCountryFlagUrl { get; }
    }
}