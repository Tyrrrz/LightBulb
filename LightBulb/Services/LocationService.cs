using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LightBulb.Models;
using Newtonsoft.Json.Linq;

namespace LightBulb.Services
{
    public class LocationService : IDisposable
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public LocationService()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                $"{App.Name} v{App.VersionString} ({App.GitHubProjectUrl})");
            _httpClient.DefaultRequestHeaders.Add("X-User-Agent-Purpose",
                $"{App.Name} is using your API to identify location of a user.");
        }

        public async Task<GeoLocation> GetLocationAsync()
        {
            var url = "http://ip-api.com/json";
            var raw = await _httpClient.GetStringAsync(url);

            var json = JToken.Parse(raw);

            var latitude = json["lat"].Value<double>();
            var longitude = json["lon"].Value<double>();

            return new GeoLocation(latitude, longitude);
        }

        public async Task<GeoLocation> GetLocationAsync(string query)
        {
            var queryEncoded = WebUtility.UrlEncode(query);

            var url = $"https://nominatim.openstreetmap.org/search?q={queryEncoded}&format=json";
            var raw = await _httpClient.GetStringAsync(url);

            var json = JToken.Parse(raw);

            var latitude = json.First["lat"].Value<double>();
            var longitude = json.First["lon"].Value<double>();

            return new GeoLocation(latitude, longitude);
        }

        public void Dispose() => _httpClient.Dispose();
    }
}