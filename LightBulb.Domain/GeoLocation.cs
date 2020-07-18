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
        private static GeoLocation? TryParseSigned(string value)
        {
            const NumberStyles numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

            // 41.25, -120.9762
            var match = Regex.Match(value, @"^([\+\-]?\d+(?:\.\d+)?)\s*[,\s]\s*([\+\-]?\d+(?:\.\d+)?)$");

            if (match.Success &&
                double.TryParse(match.Groups[1].Value, numberStyles, CultureInfo.InvariantCulture, out var lat) &&
                double.TryParse(match.Groups[2].Value, numberStyles, CultureInfo.InvariantCulture, out var lng))
            {
                return new GeoLocation(lat, lng);
            }

            return null;
        }

        private static GeoLocation? TryParseSuffixed(string value)
        {
            const NumberStyles numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

            // 41.25°N, 120.9762°W
            // 41.25N, 120.9762W
            // 41.25 N, 120.9762 W
            var match = Regex.Match(value, @"^(\d+(?:\.\d+)?)\s*°?\s*(\w)\s*[,\s]\s*(\d+(?:\.\d+)?)\s*°?\s*(\w)$");

            if (match.Success &&
                double.TryParse(match.Groups[1].Value, numberStyles, CultureInfo.InvariantCulture, out var lat) &&
                double.TryParse(match.Groups[3].Value, numberStyles, CultureInfo.InvariantCulture, out var lng))
            {
                var latSign = match.Groups[2].Value.Equals("N", StringComparison.OrdinalIgnoreCase) ? 1 : -1;
                var lngSign = match.Groups[4].Value.Equals("E", StringComparison.OrdinalIgnoreCase) ? 1 : -1;

                return new GeoLocation(lat * latSign, lng * lngSign);
            }

            return null;
        }

        public static GeoLocation? TryParse(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return
                TryParseSigned(value) ??
                TryParseSuffixed(value);
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