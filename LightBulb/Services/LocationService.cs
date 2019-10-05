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
                $"LightBulb v{App.VersionString} (github.com/Tyrrrz/LightBulb)");
            _httpClient.DefaultRequestHeaders.Add("X-User-Agent-Purpose",
                "LightBulb is using your API to identify coordinates of a user-specified location.");
        }

        public async Task<GeoLocation> GetLocationAsync()
        {
            var url = "http://ip-api.com/json";
            var raw = await _httpClient.GetStringAsync(url);

            var json = JToken.Parse(raw);

            // TODO: handle errors

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

            // TODO: handle errors

            var latitude = json.First["lat"].Value<double>();
            var longitude = json.First["lon"].Value<double>();

            return new GeoLocation(latitude, longitude);
        }

        public void Dispose() => _httpClient.Dispose();
    }
}