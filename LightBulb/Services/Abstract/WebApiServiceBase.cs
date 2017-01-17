using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LightBulb.Services.Abstract
{
    public abstract class WebApiServiceBase : IDisposable
    {
        private static readonly TimeSpan MinRequestInterval = TimeSpan.FromSeconds(0.35);
        private static DateTime _lastRequestDateTime = DateTime.MinValue;

        private readonly HttpClient _client;

        protected WebApiServiceBase()
        {
            _client = new HttpClient();
        }

        private async Task RequestThrottlingAsync()
        {
            var diff = DateTime.Now - _lastRequestDateTime;
            if (diff < MinRequestInterval)
                await Task.Delay(MinRequestInterval - diff);
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
                return null;
            }
        }

        public virtual void Dispose()
        {
            _client.Dispose();
        }
    }
}
