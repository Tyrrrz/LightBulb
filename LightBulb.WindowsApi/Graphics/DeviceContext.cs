using System.Collections.Generic;
using System.Windows.Forms;

namespace LightBulb.WindowsApi.Graphics
{
    public static class DeviceContext
    {
        private static IReadOnlyList<IDeviceContext> GetAllMonitors()
        {
            var result = new List<IDeviceContext>();

            foreach (var screen in Screen.AllScreens)
            {
                var deviceContext = SingleDeviceContext.TryGetFromDeviceName(screen.DeviceName);
                if (deviceContext != null)
                    result.Add(deviceContext);
            }

            return result;
        }

        public static IDeviceContext GetVirtualMonitor() => new AggregateDeviceContext(GetAllMonitors());
    }
}