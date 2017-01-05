using System;
using NegativeLayer.Extensions;

namespace LightBulb.Models
{
    public partial struct ColorIntensity
    {
        public double Red { get; }
        public double Green { get; }
        public double Blue { get; }

        public ColorIntensity(double red, double green, double blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public ColorIntensity(double uniform)
        {
            Red = Green = Blue = uniform;
        }

        public override string ToString()
        {
            return $"R:{Red:n3} G:{Green:n3} B:{Blue:n3}";
        }
    }

    public partial struct ColorIntensity
    {
        public static ColorIntensity FromTemperature(ushort temp)
        {
            // http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/

            double tempf = temp / 100d;

            double redi;
            double greeni;
            double bluei;

            // Red
            if (tempf <= 66)
                redi = 1;
            else
                redi = (Math.Pow(tempf - 60, -0.1332047592) * 329.698727446).Clamp(0, 255) / 255d;

            // Green
            if (tempf <= 66)
                greeni = (Math.Log(tempf) * 99.4708025861 - 161.1195681661).Clamp(0, 255) / 255d;
            else
                greeni = (Math.Pow(tempf - 60, -0.0755148492) * 288.1221695283).Clamp(0, 255) / 255d;

            // Blue
            if (tempf >= 66)
                bluei = 1;
            else
            {
                if (tempf <= 19)
                    bluei = 0;
                else
                    bluei = (Math.Log(tempf - 10) * 138.5177312231 - 305.0447927307).Clamp(0, 255) / 255d;
            }

            return new ColorIntensity(redi, greeni, bluei);
        }
    }
}