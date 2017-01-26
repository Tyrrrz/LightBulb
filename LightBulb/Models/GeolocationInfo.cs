using System.Runtime.Serialization;

namespace LightBulb.Models
{
    [DataContract]
    public class GeolocationInfo
    {
        [DataMember(Name = "country_name")]
        public string Country { get; set; }

        [DataMember(Name = "country_code")]
        public string CountryCode { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        public string CountryFlag => $"http://flags.fmcdn.net/data/flags/mini/{CountryCode.ToLowerInvariant()}.png";
    }
}
