using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Models;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components.Settings;

public partial class ApplicationWhitelistSettingsTabViewModel(
    SettingsService settingsService,
    ExternalApplicationService externalApplicationService
) : SettingsTabViewModel(settingsService, 3, "Application whitelist")
{
    [ObservableProperty]
    private IReadOnlyList<ExternalApplication>? _availableApplications;

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

    [RelayCommand]
    private void PullAvailableApplications()
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
