using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBulb.Models;
using LightBulb.Services.Abstract;
using LightBulb.Services.Interfaces;
using Tyrrrz.Extensions;
using Newtonsoft.Json;

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
                var expectedJson = new
                {
                    country_name = "",
                    country_code = "",
                    city = "",
                    latitude = 0d,
                    longitude = 0d
                };
                var parsed = JsonConvert.DeserializeAnonymousType(response, expectedJson);

                // Extract data
                var result = new GeoInfo
                {
                    Country = parsed.country_name,
                    CountryCode = parsed.country_code,
                    City = parsed.city,
                    Latitude = parsed.latitude,
                    Longitude = parsed.longitude
                };

                // Turn empty string to null (so bindings can use fallbacks)
                if (result.Country.IsBlank())
                    result.Country = null;
                if (result.City.IsBlank())
                    result.City = null;

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
                var expectedJson = new
                {
                    sunrise = DateTime.MinValue,
                    sunset = DateTime.MaxValue
                };
                var parsed = JsonConvert.DeserializeAnonymousType(response, new {results = expectedJson});

                // Extract data
                var result = new SolarInfo
                {
                    Sunrise = parsed.results.sunrise,
                    Sunset = parsed.results.sunset
                };
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