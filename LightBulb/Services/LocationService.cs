using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using LightBulb.Models;
using Newtonsoft.Json.Linq;
using Tyrrrz.Extensions;

namespace LightBulb.Services
{
    public class LocationService : IDisposable
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public LocationService()
        {
            // Set user-agent header
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"LightBulb v{version} (github.com/Tyrrrz/LightBulb)");
        }

        // TODO: move this to LightBulb's own webserver

        public async Task<GeographicCoordinates> GetLocationAsync()
        {
            var request = "http://ip-api.com/json";
            var response = await _httpClient.GetStringAsync(request);
            var responseJson = JToken.Parse(response);

            // TODO: handle errors

            var latitude = responseJson["lat"].Value<double>();
            var longitude = responseJson["lon"].Value<double>();

            return new GeographicCoordinates(latitude, longitude);
        }

        public async Task<GeographicCoordinates> GetLocationAsync(string query)
        {
            var request = $"https://nominatim.openstreetmap.org/search?q={query.UrlEncode()}&format=json";
            var response = await _httpClient.GetStringAsync(request);
            var responseJson = JToken.Parse(response);

            // TODO: handle errors

            var latitude = responseJson.First["lat"].Value<double>();
            var longitude = responseJson.First["lon"].Value<double>();

            return new GeographicCoordinates(latitude, longitude);
        }

        public void Dispose() => _httpClient.Dispose();
    }
}