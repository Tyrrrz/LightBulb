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

        /// <inheritdoc />
        public async Task<string> GetStringAsync(string url)
        {
            try
            {
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
