using System.Globalization;
using System.Text.Json;

namespace LightBulb.Core.Utils.Extensions
{
    public static class JsonExtensions
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