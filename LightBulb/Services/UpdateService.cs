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
            new ZipPackageExtractor()
        );

        private readonly SettingsService _settingsService;

        public UpdateService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        private Version? TryGetLastPreparedUpdate() => _updateManager.GetPreparedUpdates().Max();

        public async Task CheckPrepareUpdateAsync()
        {
            if (!_settingsService.IsAutoUpdateEnabled)
                return;

            try
            {
                var check = await _updateManager.CheckForUpdatesAsync();
                if (!check.CanUpdate || check.LastVersion is null)
                    return;

                if (check.LastVersion != TryGetLastPreparedUpdate())
                    await _updateManager.PrepareUpdateAsync(check.LastVersion);
            }
            catch
            {
                // Failure to check for updates shouldn't crash the app
            }
        }

        public void FinalizePendingUpdates()
        {
            if (!_settingsService.IsAutoUpdateEnabled)
                return;

            try
            {
                var lastPreparedUpdate = TryGetLastPreparedUpdate();
                if (lastPreparedUpdate is null)
                    return;

                if (lastPreparedUpdate <= _updateManager.Updatee.Version)
                    return;

                _updateManager.LaunchUpdater(lastPreparedUpdate);
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