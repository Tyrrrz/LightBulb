using System;

namespace LightBulb.WindowsApi.Graphics
{
    public interface IDeviceContext : IDisposable
    {
        void SetGamma(double redMultiplier, double greenMultiplier, double blueMultiplier);
    }
}