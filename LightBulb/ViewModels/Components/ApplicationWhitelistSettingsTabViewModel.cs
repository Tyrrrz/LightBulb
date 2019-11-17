using System;
using System.Collections.Generic;
using System.Linq;
using LightBulb.Models;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components
{
    public class ApplicationWhitelistSettingsTabViewModel :  SettingsTabViewModelBase
    {
        private readonly ExternalApplicationService _externalApplicationService;

        public bool IsApplicationWhitelistEnabled
        {
            get => SettingsService.IsApplicationWhitelistEnabled;
            set => SettingsService.IsApplicationWhitelistEnabled = value;
        }

        public IReadOnlyList<ExternalApplication> WhitelistedApplications
        {
            get => SettingsService.WhitelistedApplications ?? Array.Empty<ExternalApplication>();
            set => SettingsService.WhitelistedApplications = value;
        }

        public IReadOnlyList<ExternalApplication> RunningApplications =>
            WhitelistedApplications.Concat(_externalApplicationService.GetAllRunningApplications()
                .Where(wa => !WhitelistedApplications.Any(wb => wb.IsSameExecutableFileAs(wa)))).ToArray();

        public ApplicationWhitelistSettingsTabViewModel(SettingsService settingsService, ExternalApplicationService externalApplicationService)
            : base(settingsService, 3, "Application whitelist")
        {
            _externalApplicationService = externalApplicationService;
        }
    }
}