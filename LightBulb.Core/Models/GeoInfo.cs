namespace LightBulb.Models
{
    /// <summary>
    /// Information about geographical location
    /// </summary>
    public class GeoInfo
    {
        /// <summary>
        /// Country name
        /// </summary>
        public string Country { get; }

        /// <summary>
        /// Country code
        /// </summary>
        public string CountryCode { get; }

        /// <summary>
        /// City name
        /// </summary>
        public string City { get; }

        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; }

        public GeoInfo(string country, string countryCode, string city, double latitude, double longitude)
        {
            Country = country;
            CountryCode = countryCode;
            City = city;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}