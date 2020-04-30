using System;
using System.Collections.Generic;
using System.IO;
using LightBulb.Models;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class ExternalApplicationService
    {
        public IEnumerable<ExternalApplication> GetAllRunningApplications()
        {
            foreach (var window in SystemWindow.GetAllWindows())
            {
                using var _ = window;

                // Ignore invisible and system windows
                if (!window.IsVisible() || window.IsSystemWindow())
                    continue;

                using var process = window.GetProcess();
                var executableFilePath = process?.GetExecutableFilePath();

                // Ignore explorer
                if (string.Equals(Path.GetFileNameWithoutExtension(executableFilePath), "explorer", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!string.IsNullOrWhiteSpace(executableFilePath))
                    yield return new ExternalApplication(executableFilePath);
            }
        }

        public ExternalApplication? GetForegroundApplication()
        {
            using var window = SystemWindow.TryGetForegroundWindow();
            using var process = window?.GetProcess();

            var executableFilePath = process?.GetExecutableFilePath();

            return !string.IsNullOrWhiteSpace(executableFilePath)
                ? new ExternalApplication(executableFilePath)
                : null;
        }

        public bool IsForegroundApplicationFullScreen()
        {
            using var window = SystemWindow.TryGetForegroundWindow();
            return window != null && window.IsVisible() && !window.IsSystemWindow() && window.IsFullScreen();
        }
    }
}