using System;
using System.IO;
using System.Linq;

namespace LightBulb
{
    public partial class App
    {
        public static string Name => typeof(App).Assembly.GetName().Name;

        public static string VersionString => typeof(App).Assembly.GetName().Version.ToString(3);

        public static string GitHubProjectUrl { get; } = "https://github.com/Tyrrrz/LightBulb";

        public static string GitHubProjectReleasesUrl { get; } = "https://github.com/Tyrrrz/LightBulb/releases";

        public static string ExecutableFilePath => Path.ChangeExtension(typeof(App).Assembly.Location, "exe");

        public static bool IsStartedByUser => !Environment.GetCommandLineArgs().Contains("--autostart", StringComparer.OrdinalIgnoreCase);
    }
}