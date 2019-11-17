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

        public IReadOnlyList<ExternalApplication>? AvailableApplications { get; private set; }

        public IReadOnlyList<ExternalApplication>? WhitelistedApplications
        {
            get => SettingsService.WhitelistedApplications;
            set => SettingsService.WhitelistedApplications = value;
        }

        public ApplicationWhitelistSettingsTabViewModel(SettingsService settingsService, ExternalApplicationService externalApplicationService)
            : base(settingsService, 3, "Application whitelist")
        {
            _externalApplicationService = externalApplicationService;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            UpdateAvailableApplications();
        }

        private void UpdateAvailableApplications()
        {
            var applicationsByExecutablePath = new Dictionary<string, ExternalApplication>();

            // Add all running applications
            foreach (var application in _externalApplicationService.GetAllRunningApplications())
                applicationsByExecutablePath[application.ExecutableFilePath] = application;

            // Add previously whitelisted applications
            // (this order is important to preserve references in selected applications)
            foreach (var application in WhitelistedApplications ?? Array.Empty<ExternalApplication>())
                applicationsByExecutablePath[application.ExecutableFilePath] = application;

            AvailableApplications = applicationsByExecutablePath.Values.ToArray();
        }
    }
}