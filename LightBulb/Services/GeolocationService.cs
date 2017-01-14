using System.Threading.Tasks;
using LightBulb.Models;
using NegativeLayer.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LightBulb.Services
{
    public class GeolocationService : WebApiServiceBase
    {
        public async Task<GeolocationInfo> GetGeolocationInfoAsync()
        {
            string response = await GetStringAsync("http://freegeoip.net/json");
            if (response.IsBlank()) return null;

            return JsonConvert.DeserializeObject<GeolocationInfo>(response);
        }

        public async Task<SolarInfo> GetSolarInfoAsync(GeolocationInfo geoInfo)
        {
            string response = await GetStringAsync($"http://api.sunrise-sunset.org/json?lat={geoInfo.Latitude}&lng={geoInfo.Longitude}&formatted=0");
            if (response.IsBlank()) return null;

            return JObject.Parse(response).GetValue("results").ToObject<SolarInfo>();
        }
    }
}
