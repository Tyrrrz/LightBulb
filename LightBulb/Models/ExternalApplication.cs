using System;
using System.IO;

namespace LightBulb.Models;

public partial class ExternalApplication
{
    public string ExecutableFilePath { get; }

    public string Name => Path.GetFileNameWithoutExtension(ExecutableFilePath);

    public ExternalApplication(string executableFilePath) => ExecutableFilePath = executableFilePath;

    public override string ToString() => Name;
}

public partial class ExternalApplication
{
    private static string? NormalizeFilePath(string? filePath) =>
        !string.IsNullOrWhiteSpace(filePath)
            ? Path.GetFullPath(filePath)
            : filePath;
}

public partial class ExternalApplication : IEquatable<ExternalApplication>
{
    public bool Equals(ExternalApplication? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(
            NormalizeFilePath(ExecutableFilePath),
            NormalizeFilePath(other.ExecutableFilePath),
            StringComparison.OrdinalIgnoreCase
        );
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;

        return obj.GetType() == GetType() && Equals((ExternalApplication) obj);
    }

    public override int GetHashCode() => HashCode.Combine(NormalizeFilePath(ExecutableFilePath));

    public static bool operator ==(ExternalApplication? a, ExternalApplication? b) => a?.Equals(b) ?? false;

    public static bool operator !=(ExternalApplication? a, ExternalApplication? b) => !(a == b);
}