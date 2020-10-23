using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LightBulb.Domain.Internal
{
    internal static class Singleton
    {
        private static readonly Lazy<HttpClient> LazyHttpClient = new Lazy<HttpClient>(() =>
        {
            var handler = new HttpClientHandler();

            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            handler.UseCookies = false;

            return new HttpClient(handler, true)
            {
                DefaultRequestHeaders =
                {
                    // Required by some of the services we're using
                    UserAgent =
                    {
                        new ProductInfoHeaderValue(
                            "LightBulb",
                            typeof(Singleton).Assembly.GetName().Version?.ToString()
                        )
                    }
                }
            };
        });

        public static HttpClient HttpClient { get; } = LazyHttpClient.Value;
    }
}