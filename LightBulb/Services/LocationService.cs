using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LightBulb.Domain;
using LightBulb.Internal;

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
            const string url = "http://ip-api.com/json";
            var json = await _httpClient.GetJsonAsync(url);

            var latitude = json.GetProperty("lat").GetDouble();
            var longitude = json.GetProperty("lon").GetDouble();

            return new GeoLocation(latitude, longitude);
        }

        public async Task<GeoLocation> GetLocationAsync(string query)
        {
            var queryEncoded = WebUtility.UrlEncode(query);

            var url = $"https://nominatim.openstreetmap.org/search?q={queryEncoded}&format=json";
            var json = await _httpClient.GetJsonAsync(url);

            var firstLocationJson = json.EnumerateArray().First();

            var latitude = firstLocationJson.GetProperty("lat").GetDouble();
            var longitude = firstLocationJson.GetProperty("lon").GetDouble();

            return new GeoLocation(latitude, longitude);
        }

        public void Dispose() => _httpClient.Dispose();
    }
}