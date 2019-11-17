using System;
using System.IO;

namespace LightBulb.Models
{
    public partial class ExternalApplication
    {
        public string Name { get; }

        public string ExecutableFilePath { get; }

        public ExternalApplication(string name, string executableFilePath)
        {
            Name = name;
            ExecutableFilePath = executableFilePath;
        }

        public bool IsSameExecutableFileAs(ExternalApplication other) =>
            string.Equals(
                NormalizeFilePath(ExecutableFilePath),
                NormalizeFilePath(other.ExecutableFilePath),
                StringComparison.OrdinalIgnoreCase);
    }

    public partial class ExternalApplication
    {
        private static string? NormalizeFilePath(string? filePath) =>
            string.IsNullOrWhiteSpace(filePath) ? filePath : Path.GetFullPath(filePath);
    }
}