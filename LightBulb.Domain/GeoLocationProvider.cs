using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JsonExtensions.Http;
using LightBulb.Domain.Internal;
using LightBulb.Domain.Internal.Extensions;

namespace LightBulb.Domain
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

            var latitude = firstLocationJson.GetProperty("lat").GetDoubleOrCoerce();
            var longitude = firstLocationJson.GetProperty("lon").GetDoubleOrCoerce();

            return new GeoLocation(latitude, longitude);
        }
    }
}