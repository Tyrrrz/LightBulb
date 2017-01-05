namespace LightBulb.Models
{
    public struct ColorIntensity
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
    }
}