using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using LightBulb.Models;
using Newtonsoft.Json.Linq;
using Tyrrrz.Extensions;

namespace LightBulb.Services
{
    public class WebGeoService : IGeoService
    {
        private readonly IHttpService _httpService;

        public WebGeoService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        /// <inheritdoc />
        public async Task<GeoInfo> GetGeoInfoAsync()
        {
            var response = await _httpService.GetStringAsync("http://ip-api.com/json");
            if (response.IsBlank()) return null;

            try
            {
                // Parse
                var parsed = JToken.Parse(response);

                // Extract data
                var countryName = parsed["country"].Value<string>().NullIfBlank();
                var countryCode = parsed["countryCode"].Value<string>().NullIfBlank();
                var city = parsed["city"].Value<string>().NullIfBlank();
                var lat = parsed["lat"].Value<double>();
                var lng = parsed["lon"].Value<double>();

                // Populate
                var result = new GeoInfo(countryName, countryCode, city, lat, lng);

                return result;
            }
            catch
            {
                Debug.WriteLine("Could not deserialize geo info", GetType().Name);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<SolarInfo> GetSolarInfoAsync(GeoInfo geoInfo)
        {
            var lat = geoInfo.Latitude.ToString(CultureInfo.InvariantCulture);
            var lng = geoInfo.Longitude.ToString(CultureInfo.InvariantCulture);
            var response =
                await _httpService.GetStringAsync($"https://api.sunrise-sunset.org/json?lat={lat}&lng={lng}&formatted=0");
            if (response.IsBlank()) return null;

            try
            {
                // Parse
                var parsed = JToken.Parse(response);

                // Extract data
                var sunrise = parsed["results"]["sunrise"].Value<DateTime>();
                var sunset = parsed["results"]["sunset"].Value<DateTime>();

                // Populate
                var result = new SolarInfo(sunrise.TimeOfDay, sunset.TimeOfDay);

                return result;
            }
            catch
            {
                Debug.WriteLine("Could not deserialize solar info", GetType().Name);
                return null;
            }
        }
    }
}