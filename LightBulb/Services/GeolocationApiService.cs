using System;
using System.Net.Http;
using System.Threading.Tasks;
using LightBulb.Models;
using NegativeLayer.Extensions;
using Newtonsoft.Json;

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

            var result = JsonConvert.DeserializeObject<GeolocationInfo>(response);
            if (!result.Status.EqualsInvariant("success")) return null;

            return result;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
