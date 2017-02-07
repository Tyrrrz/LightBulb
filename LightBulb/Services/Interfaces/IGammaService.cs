using LightBulb.Models;

namespace LightBulb.Services.Interfaces
{
    /// <summary>
    /// Implemented by classes that allow control over the display gamma
    /// </summary>
    public interface IGammaService
    {
        /// <summary>
        /// Change the display gamma by multiplying each channel with the corresponding scalar
        /// </summary>
        void SetDisplayGammaLinear(ColorIntensity intensity);

        /// <summary>
        /// Restore the default display gamma
        /// </summary>
        void RestoreDefault();
    }
}