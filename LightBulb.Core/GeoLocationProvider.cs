using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JsonExtensions.Http;
using JsonExtensions.Reading;
using LightBulb.Core.Utils;

namespace LightBulb.Core
{
    public class GeoLocationProvider
    {
        private readonly HttpClient _httpClient;

        public GeoLocationProvider(HttpClient httpClient) => _httpClient = httpClient;

        public GeoLocationProvider()
            : this(Http.Client) {}

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

            var latitude = firstLocationJson.GetProperty("lat").GetDoubleCoerced();
            var longitude = firstLocationJson.GetProperty("lon").GetDoubleCoerced();

            return new GeoLocation(latitude, longitude);
        }
    }
}