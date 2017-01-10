using System;
using System.Runtime.Serialization;

namespace LightBulb.Models
{
    [DataContract]
    public class SolarInfo
    {
        [DataMember(Name = "sunrise")]
        public DateTime Sunrise { get; set; }

        [DataMember(Name = "sunset")]
        public DateTime Sunset { get; set; }
    }
}
