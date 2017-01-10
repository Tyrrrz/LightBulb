using System;
using System.Net.Http;
using System.Threading.Tasks;
using LightBulb.Models;
using NegativeLayer.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LightBulb.Services
{
    public class GeolocationApiService : IDisposable
    {
        private readonly HttpClient _client;

        public GeolocationApiService()
        {
            _client = new HttpClient();
        }

        private async Task<string> GetStringAsync(string url)
        {
            try
            {
                return await _client.GetStringAsync(url);
            }
            catch
            {
                return null;
            }
        }

        public async Task<GeolocationInfo> GetGeolocationInfoAsync()
        {
            string response = await GetStringAsync("http://ip-api.com/json");
            if (response.IsBlank()) return null;

            return JsonConvert.DeserializeObject<GeolocationInfo>(response);
        }

        public async Task<SolarInfo> GetSolarInfoAsync(double latitude, double longitude)
        {
            string response = await GetStringAsync($"http://api.sunrise-sunset.org/json?lat={latitude}&lng={longitude}&formatted=0");
            if (response.IsBlank()) return null;

            return JObject.Parse(response).GetValue("results").ToObject<SolarInfo>();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
