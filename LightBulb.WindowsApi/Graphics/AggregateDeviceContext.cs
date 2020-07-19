using System.Collections.Generic;

namespace LightBulb.WindowsApi.Graphics
{
    internal class AggregateDeviceContext : IDeviceContext
    {
        public IReadOnlyList<IDeviceContext> DeviceContexts { get; }

        public AggregateDeviceContext(IReadOnlyList<IDeviceContext> deviceContexts) =>
            DeviceContexts = deviceContexts;

        public void SetGamma(double redMultiplier, double greenMultiplier, double blueMultiplier)
        {
            foreach (var deviceContext in DeviceContexts)
                deviceContext.SetGamma(redMultiplier, greenMultiplier, blueMultiplier);
        }

        public void Dispose()
        {
            foreach (var deviceContext in DeviceContexts)
                deviceContext.Dispose();
        }
    }
}