using System;
using System.IO;

namespace LightBulb.Internal
{
    internal static class FileEx
    {
        public static bool ArePathEqual(string filePath1, string filePath2)
        {
            if (string.IsNullOrWhiteSpace(filePath1) || string.IsNullOrWhiteSpace(filePath2))
                return false;

            var normalizedPath1 = Path.GetFullPath(filePath1);
            var normalizedPath2 = Path.GetFullPath(filePath2);

            return string.Equals(normalizedPath1, normalizedPath2, StringComparison.OrdinalIgnoreCase);
        }
    }
}