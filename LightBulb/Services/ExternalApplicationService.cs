using System;
using System.Collections.Generic;
using System.IO;
using LightBulb.Models;
using LightBulb.WindowsApi.Management;

namespace LightBulb.Services
{
    public class ExternalApplicationService
    {
        public IEnumerable<ExternalApplication> GetAllRunningApplications()
        {
            foreach (var window in SystemWindow.GetAllWindows())
            {
                // Ignore invisible and system windows
                if (!window.IsVisible() || window.IsSystemWindow())
                    continue;

                using var process = window.TryGetProcess();
                var executableFilePath = process?.TryGetExecutableFilePath();

                // Ignore explorer
                if (string.Equals(Path.GetFileNameWithoutExtension(executableFilePath), "explorer", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!string.IsNullOrWhiteSpace(executableFilePath))
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