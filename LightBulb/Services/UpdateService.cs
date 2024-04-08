using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using LightBulb.Utils.Extensions;
using Onova;
using Onova.Exceptions;
using Onova.Services;

namespace LightBulb.Services;

public class UpdateService(SettingsService settingsService) : IDisposable
{
    private readonly IUpdateManager _updateManager = new UpdateManager(
        new GithubPackageResolver("Tyrrrz", "LightBulb", "LightBulb.zip"),
        new ZipPackageExtractor()
    );

    private Version? TryGetLastPreparedUpdate() => _updateManager.GetPreparedUpdates().Max();

    public async Task CheckPrepareUpdateAsync()
    {
        if (!settingsService.IsAutoUpdateEnabled)
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
        if (!settingsService.IsAutoUpdateEnabled)
            return;

        // Onova only works on Windows currently
        if (!OperatingSystem.IsWindows())
            return;

        try
        {
            var lastPreparedUpdate = TryGetLastPreparedUpdate();
            if (lastPreparedUpdate is null)
                return;

            if (lastPreparedUpdate <= _updateManager.Updatee.Version)
                return;

            _updateManager.LaunchUpdater(lastPreparedUpdate);
            
            if (Application.Current?.ApplicationLifetime?.TryShutdown(2) != true)
                Environment.Exit(2);
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
