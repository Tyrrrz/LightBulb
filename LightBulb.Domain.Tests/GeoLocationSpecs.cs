using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace LightBulb.Domain.Tests
{
    public class GeoLocationSpecs
    {
        public static IEnumerable<object?[]> GetTestCases()
        {
            // Valid

            yield return new object?[]
            {
                "41.25 -120.9762", new GeoLocation(41.25, -120.9762),
            };

            yield return new object?[]
            {
                "41.25, -120.9762", new GeoLocation(41.25, -120.9762),
            };

            yield return new object?[]
            {
                "41.25,-120.9762", new GeoLocation(41.25, -120.9762)
            };

            yield return new object?[]
            {
                "-41.25, -120.9762", new GeoLocation(-41.25, -120.9762)
            };

            yield return new object?[]
            {
                "41.25, 120.9762", new GeoLocation(41.25, 120.9762)
            };

            yield return new object?[]
            {
                "41.25°N, 120.9762°W", new GeoLocation(41.25, -120.9762)
            };

            yield return new object?[]
            {
                "41.25N 120.9762W", new GeoLocation(41.25, -120.9762)
            };

            yield return new object?[]
            {
                "41.25N, 120.9762W", new GeoLocation(41.25, -120.9762)
            };

            yield return new object?[]
            {
                "41.25 N, 120.9762 W", new GeoLocation(41.25, -120.9762)
            };

            yield return new object?[]
            {
                "41.25 S, 120.9762 W", new GeoLocation(-41.25, -120.9762)
            };

            yield return new object?[]
            {
                "41.25 S, 120.9762 E", new GeoLocation(-41.25, 120.9762)
            };

            yield return new object?[]
            {
                "41, 120", new GeoLocation(41, 120)
            };

            yield return new object?[]
            {
                "-41, -120", new GeoLocation(-41, -120)
            };

            yield return new object?[]
            {
                "41 N, 120 E", new GeoLocation(41, 120)
            };

            yield return new object?[]
            {
                "41 N, 120 W", new GeoLocation(41, -120)
            };

            // Invalid

            yield return new object?[]
            {
                "41.25; -120.9762", null
            };

            yield return new object?[]
            {
                "-41.25 S, 120.9762 E", null
            };

            yield return new object?[]
            {
                "41.25", null
            };

            yield return new object?[]
            {
                "", null
            };

            yield return new object?[]
            {
                null, null
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void I_can_type_in_my_location_using_standard_notation(
            string str, GeoLocation? expectedResult)
        {
            // Act & assert
            GeoLocation.TryParse(str).Should().Be(expectedResult);
        }
    }
}