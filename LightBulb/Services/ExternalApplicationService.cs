using System.Collections.Generic;
using LightBulb.Models;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class ExternalApplicationService
    {
        public ExternalApplication? GetForegroundApplication()
        {
            using var window = SystemWindow.GetForegroundWindow();
            using var process = window?.GetProcess();

            var executableFilePath = process?.GetExecutableFilePath();

            return !string.IsNullOrWhiteSpace(executableFilePath)
                ? new ExternalApplication(executableFilePath)
                : null;
        }

        public bool IsForegroundApplicationFullScreen()
        {
            using var window = SystemWindow.GetForegroundWindow();
            return window != null && window.IsFullScreen();
        }

        public IReadOnlyList<ExternalApplication> GetAllRunningApplications()
        {
            var result = new List<ExternalApplication>();

            foreach (var process in SystemProcess.GetAllWindowedProcesses())
            {
                using var _ = process;
                var executableFilePath = process.GetExecutableFilePath();

                if (!string.IsNullOrWhiteSpace(executableFilePath))
                    result.Add(new ExternalApplication(executableFilePath));
            }

            return result;
        }
    }
}