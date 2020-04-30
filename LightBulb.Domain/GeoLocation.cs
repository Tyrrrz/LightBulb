using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LightBulb.Domain
{
    public readonly partial struct GeoLocation
    {
        public double Latitude { get; }

        public double Longitude { get; }

        public GeoLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public override string ToString() =>
            $"{Latitude.ToString(CultureInfo.InvariantCulture)}, {Longitude.ToString(CultureInfo.InvariantCulture)}";
    }

    public partial struct GeoLocation
    {
        public static GeoLocation? TryParse(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            const NumberStyles numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

            // Signed lat/lng
            // 41.25, -120.9762
            var signedMatch = Regex.Match(value, @"^([\+\-]?\d+(?:\.\d+)?)\s*[,\s]\s*([\+\-]?\d+(?:\.\d+)?)$");
            if (signedMatch.Success)
            {
                if (double.TryParse(signedMatch.Groups[1].Value, numberStyles, CultureInfo.InvariantCulture, out var lat) &&
                    double.TryParse(signedMatch.Groups[2].Value, numberStyles, CultureInfo.InvariantCulture, out var lng))
                {
                    return new GeoLocation(lat, lng);
                }
            }

            // Suffixed lat/lng
            // 41.25°N, 120.9762°W
            // 41.25N, 120.9762W
            // 41.25 N, 120.9762 W
            var suffixedMatch = Regex.Match(value, @"^(\d+(?:\.\d+)?)\s*°?\s*(\w)\s*[,\s]\s*(\d+(?:\.\d+)?)\s*°?\s*(\w)$");
            if (suffixedMatch.Success)
            {
                if (double.TryParse(suffixedMatch.Groups[1].Value, numberStyles, CultureInfo.InvariantCulture, out var lat) &&
                    double.TryParse(suffixedMatch.Groups[3].Value, numberStyles, CultureInfo.InvariantCulture, out var lng))
                {
                    var latSign = suffixedMatch.Groups[2].Value.Equals("N", StringComparison.OrdinalIgnoreCase) ? 1 : -1;
                    var lngSign = suffixedMatch.Groups[4].Value.Equals("E", StringComparison.OrdinalIgnoreCase) ? 1 : -1;

                    return new GeoLocation(lat * latSign, lng * lngSign);
                }
            }

            return null;
        }
    }

    public partial struct GeoLocation : IEquatable<GeoLocation>
    {
        public bool Equals(GeoLocation other) => Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);

        public override bool Equals(object? obj) => obj is GeoLocation other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);

        public static bool operator ==(GeoLocation a, GeoLocation b) => a.Equals(b);

        public static bool operator !=(GeoLocation a, GeoLocation b) => !(a == b);
    }
}