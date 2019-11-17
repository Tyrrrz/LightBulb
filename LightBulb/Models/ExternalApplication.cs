using System;
using System.IO;
using Tyrrrz.Settings;

namespace LightBulb.Models
{
    public partial class ExternalApplication : IEquatable<ExternalApplication>
    {
        public string ExecutableFilePath { get; }

        [Ignore]
        public string Name => Path.GetFileNameWithoutExtension(ExecutableFilePath) ?? ExecutableFilePath;

        public ExternalApplication(string executableFilePath)
        {
            ExecutableFilePath = executableFilePath;
        }

        public bool Equals(ExternalApplication other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(
                NormalizeFilePath(ExecutableFilePath),
                NormalizeFilePath(other.ExecutableFilePath),
                StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == GetType() && Equals((ExternalApplication) obj);
        }

        public override int GetHashCode() => HashCode.Combine(NormalizeFilePath(ExecutableFilePath));
    }

    public partial class ExternalApplication
    {
        private static string? NormalizeFilePath(string? filePath) =>
            string.IsNullOrWhiteSpace(filePath) ? filePath : Path.GetFullPath(filePath);
    }
}