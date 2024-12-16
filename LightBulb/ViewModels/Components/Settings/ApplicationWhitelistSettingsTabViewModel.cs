using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightBulb.Framework;
using LightBulb.Models;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components.Settings;

public partial class ApplicationWhitelistSettingsTabViewModel : SettingsTabViewModelBase
{
    private readonly ExternalApplicationService _externalApplicationService;

    [ObservableProperty]
    private ObservableCollection<ExternalApplicationViewModel> _applications = [];

    public ApplicationWhitelistSettingsTabViewModel(
        SettingsService settingsService,
        ExternalApplicationService externalApplicationService
    )
        : base(settingsService, 3, "Application whitelist")
    {
        _externalApplicationService = externalApplicationService;
        RefreshApplications();
    }

    public bool IsApplicationWhitelistEnabled
    {
        get => SettingsService.IsApplicationWhitelistEnabled;
        set => SettingsService.IsApplicationWhitelistEnabled = value;
    }

    public bool IsApplicationBlacklistEnabled
    {
        get => SettingsService.IsApplicationBlacklistEnabled;
        set => SettingsService.IsApplicationBlacklistEnabled = value;
    }

    [RelayCommand]
    private void RefreshApplications()
    {
        Applications.Clear();

        // Add previously whitelisted applications
        // (this has to be done first to preserve references in selected applications)
        foreach (var application in SettingsService.WhitelistedApplications ?? [])
        {
            var externalAppVm = new ExternalApplicationViewModel(SettingsService)
            {
                Application = application,
                AppExclusionType = AppExclusionType.Whitelist,
            };
            Applications.Add(externalAppVm);
        }

        foreach (var application in SettingsService.BlacklistedApplications ?? [])
        {
            // There should not be duplicates at this point, but in case there were, ignore
            if (Applications.Any(x => x.Application == application))
            {
                continue;
            }

            var externalAppVm = new ExternalApplicationViewModel(SettingsService)
            {
                Application = application,
                AppExclusionType = AppExclusionType.Blacklist,
            };
            Applications.Add(externalAppVm);
        }

        // Add all running applications
        foreach (var application in _externalApplicationService.GetAllRunningApplications())
        {
            if (Applications.Any(x => x.Application == application))
            {
                continue;
            }

            var externalAppVm = new ExternalApplicationViewModel(SettingsService)
            {
                Application = application,
                AppExclusionType = AppExclusionType.None,
            };
            Applications.Add(externalAppVm);
        }
    }
}

public sealed class ExternalApplicationViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;

    public ExternalApplicationViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public required ExternalApplication Application { get; init; }

    public AppExclusionType AppExclusionType
    {
        get;
        set
        {
            var previousValue = field;
            if (previousValue == value)
            {
                return;
            }

            field = value;
            switch (value)
            {
                case AppExclusionType.None when previousValue is AppExclusionType.Whitelist:
                    RemoveFromWhitelist();
                    break;
                case AppExclusionType.None when previousValue is AppExclusionType.Blacklist:
                    RemoveFromBlacklist();
                    break;
                case AppExclusionType.Whitelist:
                    RemoveFromBlacklist();
                    AddToWhitelist();
                    break;
                case AppExclusionType.Blacklist:
                    RemoveFromWhitelist();
                    AddToBlacklist();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

    public static IReadOnlyList<AppExclusionType> AvailableExclusionTypes { get; } =
        Enum.GetValues<AppExclusionType>();

    private void RemoveFromWhitelist()
    {
        _settingsService.WhitelistedApplications = _settingsService
            .WhitelistedApplications?.Except([Application])
            .ToList();
    }

    private void RemoveFromBlacklist()
    {
        _settingsService.BlacklistedApplications = _settingsService
            .BlacklistedApplications?.Except([Application])
            .ToList();
    }

    private void AddToWhitelist()
    {
        _settingsService.WhitelistedApplications = _settingsService
            .WhitelistedApplications?.Concat([Application])
            .Distinct()
            .ToList();
    }

    private void AddToBlacklist()
    {
        _settingsService.BlacklistedApplications = _settingsService
            .BlacklistedApplications?.Concat([Application])
            .Distinct()
            .ToList();
    }
}

public enum AppExclusionType
{
    None,
    Whitelist,
    Blacklist,
}
