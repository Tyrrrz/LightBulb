namespace LightBulb.Internal
{
    internal static class EnvironmentEx
    {
        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}