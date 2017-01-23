using System.Diagnostics;
using System.Threading.Tasks;
using LightBulb.Models;
using LightBulb.Services.Abstract;
using LightBulb.Services.Helpers;
using Tyrrrz.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LightBulb.Services
{
    public class GeoSyncService : WebApiServiceBase
    {
        private readonly SyncedTimer _internetSyncTimer;

        public Settings Settings => Settings.Default;

        public GeoSyncService()
        {
            _internetSyncTimer = new SyncedTimer();
            _internetSyncTimer.Tick += async (sender, args) => await SynchronizeAsync();

            Settings.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Settings.IsInternetTimeSyncEnabled) &&
                    Settings.IsInternetTimeSyncEnabled)
                {
                    SynchronizeAsync().Forget();
                }
            };

            SynchronizeAsync().Forget();
        }

        private async Task<GeolocationInfo> GetGeolocationInfoAsync()
        {
            string response = await GetStringAsync("http://freegeoip.net/json");
            if (response.IsBlank()) return null;

            return JsonConvert.DeserializeObject<GeolocationInfo>(response);
        }

        private async Task<SolarInfo> GetSolarInfoAsync(GeolocationInfo geoInfo)
        {
            string response = await GetStringAsync($"http://api.sunrise-sunset.org/json?lat={geoInfo.Latitude}&lng={geoInfo.Longitude}&formatted=0");
            if (response.IsBlank()) return null;

            return JObject.Parse(response).GetValue("results").ToObject<SolarInfo>();
        }

        private async Task SynchronizeAsync()
        {
            Debug.WriteLine("Start internet sync", GetType().Name);

            // Get coordinates
            var geoinfo = await GetGeolocationInfoAsync();
            if (geoinfo == null) return; // fail
            Settings.GeoInfo = geoinfo;

            // Get the sunrise/sunset times
            var solarInfo = await GetSolarInfoAsync(Settings.GeoInfo);
            if (solarInfo == null) return; // fail

            // Update settings
            if (Settings.IsInternetTimeSyncEnabled)
            {
                Settings.SunriseTime = solarInfo.Sunrise.TimeOfDay;
                Settings.SunsetTime = solarInfo.Sunset.TimeOfDay;

                Debug.WriteLine("Solar info updated", GetType().Name);
            }

            Debug.WriteLine("End internet sync", GetType().Name);
        }

        public override void Dispose()
        {
            base.Dispose();
            _internetSyncTimer.Dispose();
        }
    }
}
