using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace LightBulb.Services
{
    /// <summary>
    /// Implements basic functionality for interacting with a web API
    /// </summary>
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

        ~WebApiServiceBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// If necessary, wait before executing the next request
        /// </summary>
        private async Task RequestThrottlingAsync()
        {
            var timeSinceLastRequest = DateTime.Now - _lastRequestDateTime;
            if (timeSinceLastRequest > TimeSpan.Zero && timeSinceLastRequest < _minRequestInterval)
            {
                var timeLeft = _minRequestInterval - timeSinceLastRequest;
                await Task.Delay(timeLeft);
            }
            _lastRequestDateTime = DateTime.Now;
        }

        /// <summary>
        /// Send a GET request and return response body as string
        /// </summary>
        /// <returns>Response body if request was successful, null otherwise</returns>
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client.Dispose();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
