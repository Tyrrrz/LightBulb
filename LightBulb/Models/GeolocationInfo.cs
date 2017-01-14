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

        [DataMember(Name = "region_code")]
        public string Region { get; set; }

        [DataMember(Name = "region_name")]
        public string RegionName { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "zip_code")]
        public string Zip { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "time_zone")]
        public string Timezone { get; set; }

        [DataMember(Name = "ip")]
        public string Ip { get; set; }

        public string CountryFlag => $"http://flags.fmcdn.net/data/flags/mini/{CountryCode.ToLowerInvariant()}.png";
    }
}
