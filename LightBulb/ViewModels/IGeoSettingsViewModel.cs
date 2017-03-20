using LightBulb.Services;

namespace LightBulb.ViewModels
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