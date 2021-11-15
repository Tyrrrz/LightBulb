using System.Windows.Forms;

namespace LightBulb.WindowsApi.Utils.Extensions
{
    internal static class WndProcExtensions
    {
        public static T? GetLParam<T>(this Message message)
        {
            var value = message.GetLParam(typeof(T));

            return value is not null
                ? (T) value
                : default;
        }
    }
}