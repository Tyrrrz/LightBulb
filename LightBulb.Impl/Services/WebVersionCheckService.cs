using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Tyrrrz.Extensions;

namespace LightBulb.Services
{
    public class WebVersionCheckService : WebApiServiceBase, IVersionCheckService
    {
        private static Version CurrentVersion => Assembly.GetEntryAssembly().GetName().Version;

        /// <inheritdoc />
        public async Task<bool> GetUpdateStatusAsync()
        {
            string response = await GetStringAsync("https://api.github.com/repos/Tyrrrz/LightBulb/releases");
            if (response.IsBlank()) return false;

            try
            {
                // Parse
                var parsed = JToken.Parse(response);

                // Check versions of all releases, see if any one of them is newer than the current
                foreach (var jRelease in parsed)
                {
                    string tagName = jRelease["tag_name"].Value<string>();
                    if (tagName.IsBlank()) continue;

                    Version version;
                    if (!Version.TryParse(tagName, out version)) continue;

                    if (CurrentVersion < version) return true;
                }

                return false;
            }
            catch
            {
                Debug.WriteLine("Could not deserialize github releases", GetType().Name);
                return false;
            }
        }
    }
}