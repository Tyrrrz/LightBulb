using System;
using LightBulb.Models;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class GammaService : IDisposable
    {
        private readonly GammaManager _gammaManager = new GammaManager();

        public void SetGamma(ColorTemperature temperature) =>
            _gammaManager.SetGamma(
                temperature.GetRedMultiplier(),
                temperature.GetGreenMultiplier(),
                temperature.GetBlueMultiplier());

        public void ResetGamma() => _gammaManager.SetGamma(1, 1, 1);

        public void Dispose() => _gammaManager.Dispose();
    }
}