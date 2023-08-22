using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace LightBulb.Core.Tests;

public class LocationSpecs
{
    public static TheoryData<GeoLocation, DateTimeOffset, SolarTimes> SolarTimesTestCases =>
        new()
        {
            // New York
            {
                new GeoLocation(40.7128, -74.0060),
                new DateTimeOffset(2019, 11, 04, 00, 00, 00, TimeSpan.FromHours(-5)),
                new SolarTimes(new TimeOnly(06, 30), new TimeOnly(16, 49))
            },
            // Kyiv
            {
                new GeoLocation(50.4547, 30.5238),
                new DateTimeOffset(2019, 11, 04, 00, 00, 00, TimeSpan.FromHours(+2)),
                new SolarTimes(new TimeOnly(06, 55), new TimeOnly(16, 29))
            },
            // Shenzhen
            {
                new GeoLocation(22.5333, 114.1333),
                new DateTimeOffset(2019, 11, 15, 00, 00, 00, TimeSpan.FromHours(+8)),
                new SolarTimes(new TimeOnly(06, 36), new TimeOnly(17, 40))
            },
            // Tokyo
            {
                new GeoLocation(35.6762, 139.6503),
                new DateTimeOffset(2019, 11, 04, 00, 00, 00, TimeSpan.FromHours(+9)),
                new SolarTimes(new TimeOnly(06, 05), new TimeOnly(16, 43))
            },
            // Tromso (doesn't have sunrise or sunset at this time, should instead return brightest point of day)
            {
                new GeoLocation(69.6489, 18.9551),
                new DateTimeOffset(2020, 01, 03, 00, 00, 00, TimeSpan.FromHours(+1)),
                new SolarTimes(new TimeOnly(11, 48), new TimeOnly(11, 48))
            }
        };

    [Theory]
    [MemberData(nameof(SolarTimesTestCases))]
    public void I_can_get_solar_times_based_on_my_location(
        GeoLocation location,
        DateTimeOffset instant,
        SolarTimes expectedSolarTimes
    )
    {
        // Act
        var solarTimes = SolarTimes.Calculate(location, instant);

        // Assert
        solarTimes.Sunrise
            .ToTimeSpan()
            .Should()
            .BeCloseTo(expectedSolarTimes.Sunrise.ToTimeSpan(), TimeSpan.FromMinutes(3));

        solarTimes.Sunset
            .ToTimeSpan()
            .Should()
            .BeCloseTo(expectedSolarTimes.Sunset.ToTimeSpan(), TimeSpan.FromMinutes(3));
    }

    public static TheoryData<string?, GeoLocation?> LocationParseTestCases =>
        new()
        {
            // Valid

            { "41.25 -120.9762", new GeoLocation(41.25, -120.9762) },
            { "41.25, -120.9762", new GeoLocation(41.25, -120.9762) },
            { "41.25,-120.9762", new GeoLocation(41.25, -120.9762) },
            { "-41.25, -120.9762", new GeoLocation(-41.25, -120.9762) },
            { "41.25, 120.9762", new GeoLocation(41.25, 120.9762) },
            { "41.25°N, 120.9762°W", new GeoLocation(41.25, -120.9762) },
            { "41.25N 120.9762W", new GeoLocation(41.25, -120.9762) },
            { "41.25N, 120.9762W", new GeoLocation(41.25, -120.9762) },
            { "41.25 N, 120.9762 W", new GeoLocation(41.25, -120.9762) },
            { "41.25 S, 120.9762 W", new GeoLocation(-41.25, -120.9762) },
            { "41.25 S, 120.9762 E", new GeoLocation(-41.25, 120.9762) },
            { "41, 120", new GeoLocation(41, 120) },
            { "-41, -120", new GeoLocation(-41, -120) },
            { "41 N, 120 E", new GeoLocation(41, 120) },
            { "41 N, 120 W", new GeoLocation(41, -120) },
            // Invalid

            { "41.25; -120.9762", null },
            { "-41.25 S, 120.9762 E", null },
            { "41.25", null },
            { "", null },
            { null, null }
        };

    [Theory]
    [MemberData(nameof(LocationParseTestCases))]
    public void I_can_configure_my_location_using_coordinates(
        string? str,
        GeoLocation? expectedResult
    )
    {
        // Act & assert
        GeoLocation.TryParse(str).Should().Be(expectedResult);
    }

    [Fact]
    public async Task I_can_configure_my_location_using_a_geographic_address()
    {
        // Act
        var location = await GeoLocation.SearchAsync("Kyiv, Ukraine");

        // Assert
        location.Latitude.Should().BeApproximately(50.4500, 0.01);
        location.Longitude.Should().BeApproximately(30.5241, 0.01);
    }

    [Fact]
    public async Task I_can_configure_my_location_by_inferring_it_from_my_IP_address()
    {
        // Act
        var location = await GeoLocation.GetCurrentAsync();

        // Assert
        location.Latitude.Should().NotBe(default);
        location.Longitude.Should().NotBe(default);
    }
}
