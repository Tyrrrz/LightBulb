using System.Runtime.Serialization;

namespace LightBulb.Models
{
    [DataContract]
    public class GeoInfo
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

        public string CountryFlag => $"https://cdn2.f-cdn.com/img/flags/png/{CountryCode.ToLowerInvariant()}.png";
    }
}
