namespace LightBulb
{
    public partial class App
    {
        public static string VersionString => typeof(App).Assembly.GetName().Version.ToString(3);
    }
}