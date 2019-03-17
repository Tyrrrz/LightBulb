using System.Threading.Tasks;
using Onova;
using Onova.Services;

namespace LightBulb.Services
{
    public class UpdateService
    {
        // TODO: this currently uses Onova only to check for updates and not to apply updates

        private readonly IUpdateManager _updateManager = new UpdateManager(
            new GithubPackageResolver("Tyrrrz", "LightBulb", "LightBulb*"),
            new ZipPackageExtractor());

        public async Task<bool> CheckForUpdatesAsync()
        {
            // Cleanup leftover files
            _updateManager.Cleanup();

            // Check for updates
            var check = await _updateManager.CheckForUpdatesAsync();

            return check.CanUpdate;
        }
    }
}