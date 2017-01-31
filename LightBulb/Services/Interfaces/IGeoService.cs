using System.Threading.Tasks;
using LightBulb.Models;

namespace LightBulb.Services.Interfaces
{
    /// <summary>
    /// Implemented by wrappers that can query geolocational information
    /// </summary>
    public interface IGeoService
    {
        /// <summary>
        /// Get geoinfo for current location
        /// </summary>
        Task<GeoInfo> GetGeoInfoAsync();

        /// <summary>
        /// Get solar info for given geoinfo
        /// </summary>
        Task<SolarInfo> GetSolarInfoAsync(GeoInfo geoInfo);
    }
}