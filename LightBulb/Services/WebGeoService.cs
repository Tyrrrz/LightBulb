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
    public sealed class WebGeoService : WebApiServiceBase, IGeoService
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
                (
                    parsed.country_name,
                    parsed.country_code,
                    parsed.city,
                    parsed.latitude,
                    parsed.longitude
                );

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
                var parsed = JsonConvert.DeserializeAnonymousType(response, new {results = expectedJson}).results;

                // Extract data
                var result = new SolarInfo
                (
                    parsed.sunrise.TimeOfDay,
                    parsed.sunset.TimeOfDay
                );
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