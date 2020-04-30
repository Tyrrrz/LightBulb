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

        private readonly SettingsService _settingsService;

        public UpdateService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        private Version? GetLastPreparedUpdate() => _updateManager.GetPreparedUpdates().Max();

        public async Task<Version?> CheckPrepareUpdateAsync()
        {
            if (!_settingsService.IsAutoUpdateEnabled)
                return null;

            try
            {
                var check = await _updateManager.CheckForUpdatesAsync();
                if (!check.CanUpdate || check.LastVersion == null)
                    return null;

                if (check.LastVersion != GetLastPreparedUpdate())
                    await _updateManager.PrepareUpdateAsync(check.LastVersion);

                return check.LastVersion;
            }
            catch
            {
                // Failure to check for updates shouldn't crash the app
                return null;
            }
        }

        public void FinalizePendingUpdates()
        {
            if (!_settingsService.IsAutoUpdateEnabled)
                return;

            try
            {
                var updateVersion = GetLastPreparedUpdate();
                if (updateVersion == null)
                    return;

                if (App.Version >= updateVersion)
                    return;

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