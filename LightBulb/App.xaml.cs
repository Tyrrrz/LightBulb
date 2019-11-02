using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LightBulb
{
    public partial class App
    {
        private static readonly Assembly Assembly = typeof(App).Assembly;

        public static string Name => Assembly.GetName().Name;

        public static Version Version => Assembly.GetName().Version;

        public static string VersionString => Version.ToString(3);

        public static string GitHubProjectUrl { get; } = "https://github.com/Tyrrrz/LightBulb";

        public static string GitHubProjectReleasesUrl { get; } = "https://github.com/Tyrrrz/LightBulb/releases";

        public static string ExecutableFilePath => Path.ChangeExtension(typeof(App).Assembly.Location, "exe");

        public static bool IsAutoStarted => Environment.GetCommandLineArgs().Contains("--autostart", StringComparer.OrdinalIgnoreCase);
    }
}