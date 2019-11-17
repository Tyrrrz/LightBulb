using System.Collections.Generic;
using System.IO;
using LightBulb.Models;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public partial class ExternalApplicationService
    {
        public ExternalApplication GetForegroundApplication()
        {
            using var window = SystemWindow.GetForegroundWindow();
            using var process = window.GetProcess();

            return GetExternalApplication(process);
        }

        public bool IsForegroundApplicationFullScreen()
        {
            using var window = SystemWindow.GetForegroundWindow();
            return window.IsFullScreen();
        }

        public IReadOnlyList<ExternalApplication> GetAllRunningApplications()
        {
            var result = new List<ExternalApplication>();

            foreach (var process in SystemProcess.GetAllWindowedProcesses())
            {
                using (process)
                {
                    var executableFilePath = process.GetExecutableFilePath();

                    if (!string.IsNullOrWhiteSpace(executableFilePath))
                        result.Add(GetExternalApplication(process));
                }
            }

            return result;
        }
    }

    public partial class ExternalApplicationService
    {
        private static ExternalApplication GetExternalApplication(SystemProcess process)
        {
            var executableFilePath = process.GetExecutableFilePath();
            var name = Path.GetFileNameWithoutExtension(executableFilePath) ?? executableFilePath;
            return new ExternalApplication(name, executableFilePath);
        }
    }
}