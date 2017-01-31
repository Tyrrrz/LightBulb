using System.Threading.Tasks;
using LightBulb.Models;
using LightBulb.Services.Abstract;
using LightBulb.Services.Interfaces;
using Tyrrrz.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LightBulb.Services
{
    public class WebGeoService : WebApiServiceBase, IGeoService
    {
        public async Task<GeoInfo> GetGeoInfoAsync()
        {
            string response = await GetStringAsync("http://freegeoip.net/json");
            if (response.IsBlank()) return null;

            var result = JsonConvert.DeserializeObject<GeoInfo>(response);
            if (result.Country.IsBlank())
                result.Country = null;
            if (result.City.IsBlank())
                result.City = null;

            return result;
        }

        public async Task<SolarInfo> GetSolarInfoAsync(GeoInfo geoInfo)
        {
            string response = await GetStringAsync($"http://api.sunrise-sunset.org/json?lat={geoInfo.Latitude}&lng={geoInfo.Longitude}&formatted=0");
            if (response.IsBlank()) return null;

            return JObject.Parse(response).GetValue("results").ToObject<SolarInfo>();
        }
    }
}