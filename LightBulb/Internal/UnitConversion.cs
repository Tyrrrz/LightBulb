using System;

namespace LightBulb.Internal
{
    internal static class UnitConversion
    {
        public static double DegreesToRadians(double degree) => degree * (Math.PI / 180);

        public static double RadiansToDegrees(double radians) => radians * 180 / Math.PI;
    }
}