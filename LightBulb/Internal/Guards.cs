using System;

namespace LightBulb.Internal
{
    internal static class Guards
    {
        public static double GuardNotNegative(this double i, string argName = null)
        {
            return i >= 0
                ? i
                : throw new ArgumentOutOfRangeException(argName, i, "Cannot be negative.");
        }
    }
}