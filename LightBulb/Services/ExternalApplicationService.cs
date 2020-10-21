using System;
using System.Collections.Generic;
using System.IO;
using LightBulb.Models;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class ExternalApplicationService
    {
        // Applications that we don't to show to the user
        private readonly HashSet<string> _ignoredApplicationNames =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "explorer"
            };

        public IEnumerable<ExternalApplication> GetAllRunningApplications()
        {
            foreach (var window in SystemWindow.GetAllWindows())
            {
                if (!window.IsVisible() || window.IsSystemWindow())
                {
                    continue;
                }

                using var process = window.TryGetProcess();

                var executableFilePath = process?.TryGetExecutableFilePath();
                var executableFileName = Path.GetFileNameWithoutExtension(executableFilePath);

                if (string.IsNullOrWhiteSpace(executableFilePath) || string.IsNullOrWhiteSpace(executableFileName))
                {
                    continue;
                }

                if (_ignoredApplicationNames.Contains(executableFileName))
                {
                    continue;
                }

                yield return new ExternalApplication(executableFilePath);
            }
        }

        public ExternalApplication? TryGetForegroundApplication()
        {
            var window = SystemWindow.TryGetForegroundWindow();
            using var process = window?.TryGetProcess();

            var executableFilePath = process?.TryGetExecutableFilePath();

            return !string.IsNullOrWhiteSpace(executableFilePath)
                ? new ExternalApplication(executableFilePath)
                : null;
        }

        public bool IsForegroundApplicationFullScreen()
        {
            var window = SystemWindow.TryGetForegroundWindow();

            return
                window != null &&
                window.IsVisible() &&
                !window.IsSystemWindow() &&
                window.IsFullScreen();
        }
    }
}