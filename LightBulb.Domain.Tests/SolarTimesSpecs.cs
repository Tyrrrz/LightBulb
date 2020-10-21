using System;
using FluentAssertions;
using Xunit;

namespace LightBulb.Domain.Tests
{
    public class SolarTimesSpecs
    {
        public static TheoryData<GeoLocation, DateTimeOffset, SolarTimes> SolarTimesTestCases =>
            new TheoryData<GeoLocation, DateTimeOffset, SolarTimes>
            {
                // New York
                {
                    new GeoLocation(40.7128, -74.0060),
                    new DateTimeOffset(2019, 11, 04, 00, 00, 00, TimeSpan.FromHours(-5)),
                    new SolarTimes(
                        new TimeOfDay(06, 30),
                        new TimeOfDay(16, 49)
                    )
                },

                // Kyiv
                {
                    new GeoLocation(50.4547, 30.5238),
                    new DateTimeOffset(2019, 11, 04, 00, 00, 00, TimeSpan.FromHours(+2)),
                    new SolarTimes(
                        new TimeOfDay(06, 55),
                        new TimeOfDay(16, 29)
                    )
                },

                // Shenzhen
                {
                    new GeoLocation(22.5333, 114.1333),
                    new DateTimeOffset(2019, 11, 15, 00, 00, 00, TimeSpan.FromHours(+8)),
                    new SolarTimes(
                        new TimeOfDay(06, 36),
                        new TimeOfDay(17, 40)
                    )
                },

                // Tokyo
                {
                    new GeoLocation(35.6762, 139.6503),
                    new DateTimeOffset(2019, 11, 04, 00, 00, 00, TimeSpan.FromHours(+9)),
                    new SolarTimes(
                        new TimeOfDay(06, 05),
                        new TimeOfDay(16, 43)
                    )
                },

                // Tromso (doesn't have sunrise or sunset at this time, should instead return brightest point of day)
                {
                    new GeoLocation(69.6489, 18.9551),
                    new DateTimeOffset(2020, 01, 03, 00, 00, 00, TimeSpan.FromHours(+1)),
                    new SolarTimes(
                        new TimeOfDay(11, 48),
                        new TimeOfDay(11, 48)
                    )
                }
            };

        [Theory]
        [MemberData(nameof(SolarTimesTestCases))]
        public void I_should_be_able_to_calculate_solar_times_based_on_my_location(
            GeoLocation location, DateTimeOffset instant, SolarTimes expectedSolarTimes)
        {
            // Act
            var solarTimes = SolarTimes.Calculate(location, instant);

            // Assert
            solarTimes.Sunrise.AsTimeSpan().Should()
                .BeCloseTo(expectedSolarTimes.Sunrise.AsTimeSpan(), TimeSpan.FromMinutes(3));

            solarTimes.Sunset.AsTimeSpan().Should()
                .BeCloseTo(expectedSolarTimes.Sunset.AsTimeSpan(), TimeSpan.FromMinutes(3));
        }
    }
}