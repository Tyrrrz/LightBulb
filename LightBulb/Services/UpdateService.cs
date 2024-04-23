using System;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<Version?> CheckForUpdatesAsync()
    {
        if (!settingsService.IsAutoUpdateEnabled)
            return null;

        var check = await _updateManager.CheckForUpdatesAsync();
        return check.CanUpdate ? check.LastVersion : null;
    }

    public Version? TryGetLastPreparedUpdate()
    {
        if (!settingsService.IsAutoUpdateEnabled)
            return null;

        var version = _updateManager.GetPreparedUpdates().Max();
        if (version <= _updateManager.Updatee.Version)
            return null;

        return version;
    }

    public async Task PrepareUpdateAsync(Version version)
    {
        if (!settingsService.IsAutoUpdateEnabled)
            return;

        try
        {
            if (version == TryGetLastPreparedUpdate())
                return;

            await _updateManager.PrepareUpdateAsync(version);
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

    public void FinalizeUpdate(Version version)
    {
        if (!settingsService.IsAutoUpdateEnabled)
            return;

        // Onova only works on Windows currently
        if (!OperatingSystem.IsWindows())
            return;

        try
        {
            _updateManager.LaunchUpdater(version);
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
