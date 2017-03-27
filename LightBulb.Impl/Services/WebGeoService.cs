using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBulb.Models;
using Newtonsoft.Json.Linq;
using Tyrrrz.Extensions;

namespace LightBulb.Services
{
    public class WebGeoService : WebApiServiceBase, IGeoService
    {
        /// <inheritdoc />
        public async Task<GeoInfo> GetGeoInfoAsync()
        {
            string response = await GetStringAsync("http://freegeoip.net/json");
            if (response.IsBlank()) return null;

            try
            {
                // Parse
                var parsed = JObject.Parse(response);

                // Extract data
                string countryName = parsed["country_name"].Value<string>().NullIfBlank();
                string countryCode = parsed["country_code"].Value<string>().NullIfBlank();
                string city = parsed["city"].Value<string>().NullIfBlank();
                double lat = parsed["latitude"].Value<double>();
                double lng = parsed["longitude"].Value<double>();

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
            double lat = geoInfo.Latitude;
            double lng = geoInfo.Longitude;
            string response =
                await GetStringAsync($"http://api.sunrise-sunset.org/json?lat={lat}&lng={lng}&formatted=0");
            if (response.IsBlank()) return null;

            try
            {
                // Parse
                var parsed = JObject.Parse(response);

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