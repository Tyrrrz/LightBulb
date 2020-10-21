using System.Windows.Forms;

namespace LightBulb.WindowsApi.Internal.Extensions
{
    internal static class WndProcExtensions
    {
        public static T GetLParam<T>(this Message message) =>
            (T) message.GetLParam(typeof(T));
    }
}