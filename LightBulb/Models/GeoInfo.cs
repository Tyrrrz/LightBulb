using Tyrrrz.Extensions;

namespace LightBulb.Models
{
    public class GeoInfo
    {
        public string Country { get; set; }

        public string CountryCode { get; set; }

        public string City { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string CountryFlagUrl => CountryCode.IsNotBlank()
            ? $"https://cdn2.f-cdn.com/img/flags/png/{CountryCode.ToLowerInvariant()}.png"
            : "https://cdn2.f-cdn.com/img/flags/png/unknown.png";
    }
}
