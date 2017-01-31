using System;
using System.Reflection;
using System.Threading.Tasks;
using LightBulb.Services.Abstract;
using LightBulb.Services.Interfaces;
using Newtonsoft.Json.Linq;
using Tyrrrz.Extensions;

namespace LightBulb.Services
{
    public class GithubVersionCheckService : WebApiServiceBase, IVersionCheckService
    {
        /// <inheritdoc />
        public async Task<bool> GetUpdateStatusAsync()
        {
            string response = await GetStringAsync("https://api.github.com/repos/Tyrrrz/LightBulb/releases");
            if (response.IsBlank()) return false;

            var releases = JArray.Parse(response);
            string newestVersionStr = (releases.First as JObject).GetValue("tag_name").Value<string>();
            if (newestVersionStr.IsBlank()) return false;

            Version newestVersion;
            if (!Version.TryParse(newestVersionStr, out newestVersion)) return false;

            return newestVersion > Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}