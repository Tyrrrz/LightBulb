using System;

namespace LightBulb.Models
{
    public partial struct GeographicCoordinates : IEquatable<GeographicCoordinates>
    {
        public double Latitude { get; }

        public double Longitude { get; }

        public GeographicCoordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public bool Equals(GeographicCoordinates other) =>
            Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            return obj is GeographicCoordinates other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Latitude.GetHashCode() * 397) ^ Longitude.GetHashCode();
            }
        }
    }

    public partial struct GeographicCoordinates
    {
        public static bool operator ==(GeographicCoordinates a, GeographicCoordinates b) => a.Equals(b);

        public static bool operator !=(GeographicCoordinates a, GeographicCoordinates b) => !(a == b);
    }
}