using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LightBulb.Domain.Internal.Extensions
{
    internal static class HttpClientExtensions
    {
        public static async Task<JsonElement> ReadAsJsonAsync(this HttpContent content)
        {
            await using var stream = await content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            return doc.RootElement.Clone();
        }

        public static async Task<JsonElement> GetJsonAsync(this HttpClient httpClient, string uri)
        {
            using var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            return await response.Content.ReadAsJsonAsync();
        }
    }
}