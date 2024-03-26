using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Models;
using LightBulb.Services;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;

namespace LightBulb.ViewModels.Components.Settings;

public partial class ApplicationWhitelistSettingsTabViewModel : SettingsTabViewModelBase
{
    private readonly ExternalApplicationService _externalApplicationService;
    private readonly DisposablePool _disposablePool = new();

    [ObservableProperty]
    private IReadOnlyList<ExternalApplication>? _availableApplications;

    public ApplicationWhitelistSettingsTabViewModel(
        SettingsService settingsService,
        ExternalApplicationService externalApplicationService
    )
        : base(settingsService, 3, "Application whitelist")
    {
        _externalApplicationService = externalApplicationService;

        _disposablePool.Add(
            this.WatchProperty(
                o => o.IsApplicationWhitelistEnabled,
                () => PullAvailableApplicationsCommand.NotifyCanExecuteChanged()
            )
        );
    }

    public bool IsApplicationWhitelistEnabled
    {
        get => SettingsService.IsApplicationWhitelistEnabled;
        set => SettingsService.IsApplicationWhitelistEnabled = value;
    }

    public IReadOnlyList<ExternalApplication>? WhitelistedApplications
    {
        get => SettingsService.WhitelistedApplications;
        set => SettingsService.WhitelistedApplications = value;
    }

    private bool CanPullAvailableApplications() => IsApplicationWhitelistEnabled;

    [RelayCommand(CanExecute = nameof(CanPullAvailableApplications))]
    private void PullAvailableApplications()
    {
        var applications = new HashSet<ExternalApplication>();

        // Add previously whitelisted applications
        // (this has to be done first to preserve references in selected applications)
        foreach (var application in WhitelistedApplications ?? Array.Empty<ExternalApplication>())
            applications.Add(application);

        // Add all running applications
        foreach (var application in _externalApplicationService.GetAllRunningApplications())
            applications.Add(application);

        AvailableApplications = applications.ToArray();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposablePool.Dispose();
        }

        base.Dispose(disposing);
    }
}
