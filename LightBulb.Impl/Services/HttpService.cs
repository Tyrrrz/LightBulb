using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LightBulb.Services
{
    /// <summary>
    /// Implements basic HTTP handler with throttling
    /// </summary>
    public class HttpService : IHttpService, IDisposable
    {
        private readonly HttpClient _client;

        private readonly TimeSpan _minRequestInterval = TimeSpan.FromSeconds(0.35);
        private DateTime _lastRequestDateTime = DateTime.MinValue;

        public HttpService()
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Add("User-Agent", "LightBulb (github.com/Tyrrrz/LightBulb)");
        }

        ~HttpService()
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

        /// <inheritdoc />
        public async Task<string> GetStringAsync(string url)
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
