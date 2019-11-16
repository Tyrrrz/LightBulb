namespace LightBulb.Models
{
    public class ExcludedApplication
    {
        public string ExecutableFilePath { get; }

        public ExcludedApplication(string executableFilePath)
        {
            ExecutableFilePath = executableFilePath;
        }
    }
}