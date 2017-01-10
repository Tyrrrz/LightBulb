using System.Runtime.Serialization;

namespace LightBulb.Models
{
    [DataContract]
    public class GeolocationInfo
    {
        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "countryCode")]
        public string CountryCode { get; set; }

        [DataMember(Name = "region")]
        public string Region { get; set; }

        [DataMember(Name = "regionName")]
        public string RegionName { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "zip")]
        public string Zip { get; set; }

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lon")]
        public double Longitude { get; set; }

        [DataMember(Name = "timezone")]
        public string Timezone { get; set; }

        [DataMember(Name = "isp")]
        public string InternetServiceProvider { get; set; }

        [DataMember(Name = "org")]
        public string OrganizationName { get; set; }

        [DataMember(Name = "as")]
        public string AutonomousSystemName { get; set; }

        [DataMember(Name = "query")]
        public string Ip { get; set; }
    }
}
