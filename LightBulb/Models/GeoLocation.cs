using System;

namespace LightBulb.Models
{
    public partial struct GeoLocation : IEquatable<GeoLocation>
    {
        public double Latitude { get; }

        public double Longitude { get; }

        public GeoLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public bool Equals(GeoLocation other) =>
            Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            return obj is GeoLocation other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Latitude.GetHashCode() * 397) ^ Longitude.GetHashCode();
            }
        }

        public override string ToString() => $"Lat: {Latitude} / Lng: {Longitude}";
    }

    public partial struct GeoLocation
    {
        public static bool operator ==(GeoLocation a, GeoLocation b) => a.Equals(b);

        public static bool operator !=(GeoLocation a, GeoLocation b) => !(a == b);
    }
}