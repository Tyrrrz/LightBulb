using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JsonExtensions.Http;
using JsonExtensions.Reading;
using LightBulb.Core.Utils;

namespace LightBulb.Core;

public readonly partial record struct GeoLocation(double Latitude, double Longitude)
{
    public override string ToString() =>
        $"{Latitude.ToString(CultureInfo.InvariantCulture)}, {Longitude.ToString(CultureInfo.InvariantCulture)}";
}

public partial record struct GeoLocation
{
    private static GeoLocation? TryParseSigned(string value)
    {
        const NumberStyles numberStyles =
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

        // 41.25, -120.9762
        var match = Regex.Match(
            value,
            @"^([\+\-]?\d+(?:\.\d+)?)\s*[,\s]\s*([\+\-]?\d+(?:\.\d+)?)$"
        );

        if (
            match.Success
            && double.TryParse(
                match.Groups[1].Value,
                numberStyles,
                CultureInfo.InvariantCulture,
                out var lat
            )
            && double.TryParse(
                match.Groups[2].Value,
                numberStyles,
                CultureInfo.InvariantCulture,
                out var lng
            )
        )
        {
            return new GeoLocation(lat, lng);
        }

        return null;
    }

    private static GeoLocation? TryParseSuffixed(string value)
    {
        const NumberStyles numberStyles =
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

        // 41.25°N, 120.9762°W
        // 41.25N, 120.9762W
        // 41.25 N, 120.9762 W
        var match = Regex.Match(
            value,
            @"^(\d+(?:\.\d+)?)\s*°?\s*(\w)\s*[,\s]\s*(\d+(?:\.\d+)?)\s*°?\s*(\w)$"
        );

        if (
            match.Success
            && double.TryParse(
                match.Groups[1].Value,
                numberStyles,
                CultureInfo.InvariantCulture,
                out var lat
            )
            && double.TryParse(
                match.Groups[3].Value,
                numberStyles,
                CultureInfo.InvariantCulture,
                out var lng
            )
        )
        {
            var latSign = match.Groups[2].Value.Equals("N", StringComparison.OrdinalIgnoreCase)
                ? 1
                : -1;
            var lngSign = match.Groups[4].Value.Equals("E", StringComparison.OrdinalIgnoreCase)
                ? 1
                : -1;

            return new GeoLocation(lat * latSign, lng * lngSign);
        }

        return null;
    }

    public static GeoLocation? TryParse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return TryParseSigned(value) ?? TryParseSuffixed(value);
    }

    public static async Task<GeoLocation> GetCurrentAsync()
    {
        // HTTPS isn't supported by this endpoint
        const string url = "http://ip-api.com/json";
        var json = await Http.Client.GetJsonAsync(url);

        var latitude = json.GetProperty("lat").GetDouble();
        var longitude = json.GetProperty("lon").GetDouble();

        return new GeoLocation(latitude, longitude);
    }

    public static async Task<GeoLocation> SearchAsync(string query)
    {
        var queryEncoded = Uri.EscapeDataString(query);

        var url = $"https://nominatim.openstreetmap.org/search?q={queryEncoded}&format=json";
        var json = await Http.Client.GetJsonAsync(url);

        var firstLocationJson = json.EnumerateArray().First();

        var latitude = firstLocationJson.GetProperty("lat").GetDoubleCoerced();
        var longitude = firstLocationJson.GetProperty("lon").GetDoubleCoerced();

        return new GeoLocation(latitude, longitude);
    }
}
