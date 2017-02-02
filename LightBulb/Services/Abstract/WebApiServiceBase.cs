using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace LightBulb.Services.Abstract
{
    public abstract class WebApiServiceBase : IDisposable
    {
        private readonly HttpClient _client;

        private readonly TimeSpan _minRequestInterval = TimeSpan.FromSeconds(0.35);
        private DateTime _lastRequestDateTime = DateTime.MinValue;

        protected WebApiServiceBase()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("User-Agent", "LightBulb (github.com/Tyrrrz/LightBulb)");
        }

        private async Task RequestThrottlingAsync()
        {
            var diff = DateTime.Now - _lastRequestDateTime;
            if (diff < _minRequestInterval)
            {
                var timeLeft = _minRequestInterval - diff;
                if (timeLeft < TimeSpan.Zero || timeLeft > _minRequestInterval) // sanity check in case system time fucks up
                    timeLeft = _minRequestInterval;
                await Task.Delay(timeLeft);
            }
            _lastRequestDateTime = DateTime.Now;
        }

        protected async Task<string> GetStringAsync(string url)
        {
            try
            {
                await RequestThrottlingAsync();
                return await _client.GetStringAsync(url);
            }
            catch
            {
                Debug.WriteLine($"Get request failed ({url})", GetType().Name);
                return null;
            }
        }

        public virtual void Dispose()
        {
            _client.Dispose();
        }
    }
}
