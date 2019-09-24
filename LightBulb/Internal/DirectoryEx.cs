using System;
using System.IO;

namespace LightBulb.Internal
{
    internal static class DirectoryEx
    {
        public static bool CheckWriteAccess(string dirPath)
        {
            var testFilePath = Path.Combine(dirPath, Guid.NewGuid().ToString());

            try
            {
                File.WriteAllText(testFilePath, "");
                File.Delete(testFilePath);

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}