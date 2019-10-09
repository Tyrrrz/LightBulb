using System;
using System.Linq;
using System.Threading.Tasks;
using Onova;
using Onova.Exceptions;
using Onova.Services;

namespace LightBulb.Services
{
    public class UpdateService : IDisposable
    {
        private readonly IUpdateManager _updateManager = new UpdateManager(
            new GithubPackageResolver("Tyrrrz", "LightBulb", "LightBulb.zip"),
            new ZipPackageExtractor());

        private Version GetLastPreparedUpdate() => _updateManager.GetPreparedUpdates().Max();

        public async Task<Version> CheckPrepareUpdateAsync()
        {
            try
            {
                // Check for updates
                var check = await _updateManager.CheckForUpdatesAsync();
                if (!check.CanUpdate)
                    return null;

                // Prepare update
                if (check.LastVersion != GetLastPreparedUpdate())
                    await _updateManager.PrepareUpdateAsync(check.LastVersion);

                return check.LastVersion;
            }
            catch (UpdaterAlreadyLaunchedException)
            {
                // Ignore race conditions
                return null;
            }
            catch (LockFileNotAcquiredException)
            {
                // Ignore race conditions
                return null;
            }
        }

        public void FinalizePendingUpdates()
        {
            try
            {
                // Get last prepared update
                var updateVersion = GetLastPreparedUpdate();
                if (updateVersion == null)
                    return;

                // Launch updater and restart
                _updateManager.LaunchUpdater(updateVersion);
                Environment.Exit(0);
            }
            catch (UpdaterAlreadyLaunchedException)
            {
                // Ignore race conditions
            }
            catch (LockFileNotAcquiredException)
            {
                // Ignore race conditions
            }
        }

        public void Dispose() => _updateManager.Dispose();
    }
}