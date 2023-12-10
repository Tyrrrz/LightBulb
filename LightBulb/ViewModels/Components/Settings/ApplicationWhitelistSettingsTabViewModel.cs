using System;
using System.Collections.Generic;
using System.Linq;
using LightBulb.Models;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components.Settings;

public class ApplicationWhitelistSettingsTabViewModel(
    SettingsService settingsService,
    ExternalApplicationService externalApplicationService
) : SettingsTabViewModelBase(settingsService, 3, "Application whitelist")
{
    public bool IsApplicationWhitelistEnabled
    {
        get => SettingsService.IsApplicationWhitelistEnabled;
        set => SettingsService.IsApplicationWhitelistEnabled = value;
    }

    public IReadOnlyList<ExternalApplication>? AvailableApplications { get; private set; }

    public IReadOnlyList<ExternalApplication>? WhitelistedApplications
    {
        get => SettingsService.WhitelistedApplications;
        set => SettingsService.WhitelistedApplications = value;
    }

    public void OnViewLoaded() => PullAvailableApplications();

    public void PullAvailableApplications()
    {
        var applications = new HashSet<ExternalApplication>();

        // Add previously whitelisted applications
        // (this has to be first to preserve references in selected applications)
        foreach (var application in WhitelistedApplications ?? Array.Empty<ExternalApplication>())
            applications.Add(application);

        // Add all running applications
        foreach (var application in externalApplicationService.GetAllRunningApplications())
            applications.Add(application);

        AvailableApplications = applications.ToArray();
    }
}
