using System.Globalization;
using System.Text.Json;

namespace LightBulb.Domain.Internal.Extensions
{
    internal static class JsonExtensions
    {
        public static double GetDoubleOrCoerce(this JsonElement element)
        {
            // If it's a string, parse
            if (element.ValueKind == JsonValueKind.String)
                return double.Parse(element.GetString(), CultureInfo.InvariantCulture);

            // Otherwise, try to read as number
            return element.GetDouble();
        }
    }
}