using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBulb.Models;
using LightBulb.Services;

namespace LightBulb.ViewModels.Components.Settings
{
    public class ApplicationWhitelistSettingsTabViewModel : SettingsTabViewModelBase
    {
        private readonly ExternalApplicationService _externalApplicationService;

        public bool IsBusy { get; private set; }

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

        public void OnViewLoaded() => UpdateAvailableApplications();

        private async void UpdateAvailableApplications()
        {
            IsBusy = true;

            try
            {
                // Getting a list of all processes is somehow pretty CPU-intensive so we offload this to a separate thread
                await Task.Run(() =>
                {
                    var applications = new HashSet<ExternalApplication>();

                    // Add previously whitelisted applications
                    // (this has to be first to preserve references in selected applications)
                    foreach (var application in WhitelistedApplications ?? Array.Empty<ExternalApplication>())
                        applications.Add(application);

                    // Add all running applications
                    foreach (var application in _externalApplicationService.GetAllRunningApplications())
                        applications.Add(application);

                    AvailableApplications = applications.ToArray();
                });
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}