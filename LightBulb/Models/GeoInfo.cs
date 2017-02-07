using Tyrrrz.Extensions;

namespace LightBulb.Models
{
    public class GeoInfo
    {
        public string Country { get; }

        public string CountryCode { get; }

        public string City { get; }

        public double Latitude { get; }

        public double Longitude { get; }

        public GeoInfo(string country, string countryCode, string city, double latitude, double longitude)
        {
            Country = country;
            CountryCode = countryCode;
            City = city;
            Latitude = latitude;
            Longitude = longitude;

            if (Country.IsBlank()) Country = null;
            if (CountryCode.IsBlank()) CountryCode = null;
            if (City.IsBlank()) City = null;
        }
    }
}