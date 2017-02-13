using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LightBulb.Services.Abstract
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
            HttpClientHandler httpClientHandler = new HttpClientHandler();

            try
            {
                httpClientHandler.Proxy = GetEnvironmentProxy();                
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Error during proxy parsing. " + exc.Message);                
            }            

            _client = new HttpClient(httpClientHandler, true);
            _client.DefaultRequestHeaders.Add("User-Agent", "LightBulb (github.com/Tyrrrz/LightBulb)");            
        }

        /// <summary>
        /// Check for the presence of HTTP_PROXY environment variable.
        /// The proxy value should follow the pattern : [http://][user:password@]host[:port]
        /// Example : http://192.168.0.1 or john:doe@theproxy.net:3128
        /// </summary>
        /// <returns>IWebProxy if valid proxy found; otherwise null. </returns>
        private IWebProxy GetEnvironmentProxy()
        {
            IWebProxy proxy = null;
            var envProxy = Environment.GetEnvironmentVariable("HTTP_PROXY");
            if (envProxy != null)
            {
                proxy = new WebProxy(envProxy);

                if (envProxy.StartsWith("http://"))
                {
                    envProxy = envProxy.Remove(0, 7);
                }

                if (envProxy.Contains("@"))
                {
                    var tmp = envProxy.Split('@');
                    var auth = tmp[0].Split(':');
                    proxy.Credentials = new NetworkCredential(auth[0], auth[1]);
                }
            }

            return proxy;
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

        public virtual void Dispose()
        {
            _client.Dispose();
        }
    }
}
