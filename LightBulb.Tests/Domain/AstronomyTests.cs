using System;
using System.Collections.Generic;
using LightBulb.Domain;
using LightBulb.Models;
using NUnit.Framework;

namespace LightBulb.Tests.Domain
{
    [TestFixture]
    public class AstronomyTests
    {
        private static IEnumerable<TestCaseData> GetTestCases_CalculateSunriseStartTime()
        {
            // Kyiv
            yield return new TestCaseData(
                new GeoLocation(50.4547, 30.5238),
                new DateTimeOffset(2020, 04, 02, 00, 00, 00, TimeSpan.FromHours(+3)),
                new TimeSpan(05, 57, 00)
            );
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases_CalculateSunriseStartTime))]
        public void CalculateSunriseStartTime_Test(GeoLocation location, DateTimeOffset instant, TimeSpan expected)
        {
            // Act
            var sunriseTime = Astronomy.CalculateSunriseStartTime(location, instant);

            // Assert
            Assert.That(sunriseTime, Is.EqualTo(expected).Within(TimeSpan.FromMinutes(3)));
        }

        private static IEnumerable<TestCaseData> GetTestCases_CalculateSunriseEndTime()
        {
            // New York
            yield return new TestCaseData(
                new GeoLocation(40.7128, -74.0060),
                new DateTimeOffset(2019, 11, 04, 00, 00, 00, TimeSpan.FromHours(-5)),
                new TimeSpan(06, 30, 00)
            );

            // Kyiv
            yield return new TestCaseData(
                new GeoLocation(50.4547, 30.5238),
                new DateTimeOffset(2019, 11, 04, 00, 00, 00, TimeSpan.FromHours(+2)),
                new TimeSpan(06, 55, 00)
            );

            // Shenzhen
            yield return new TestCaseData(
                new GeoLocation(22.5333, 114.1333),
                new DateTimeOffset(2019, 11, 15, 00, 00, 00, TimeSpan.FromHours(+8)),
                new TimeSpan(06, 36, 00)
            );

            // Tromso (doesn't have sunrise at this time, should instead return brightest point of day)
            yield return new TestCaseData(
                new GeoLocation(69.6489, 18.9551),
                new DateTimeOffset(2020, 01, 03, 00, 00, 00, TimeSpan.FromHours(+1)),
                new TimeSpan(11, 48, 00)
            );
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases_CalculateSunriseEndTime))]
        public void CalculateSunriseEndTime_Test(GeoLocation location, DateTimeOffset instant, TimeSpan expected)
        {
            // Act
            var sunriseTime = Astronomy.CalculateSunriseEndTime(location, instant);

            // Assert
            Assert.That(sunriseTime, Is.EqualTo(expected).Within(TimeSpan.FromMinutes(3)));
        }

        private static IEnumerable<TestCaseData> GetTestCases_CalculateSunsetStartTime()
        {
            // New York
            yield return new TestCaseData(
                new GeoLocation(40.7128, -74.0060),
                new DateTimeOffset(2019, 11, 04, 00, 00, 00, TimeSpan.FromHours(-5)),
                new TimeSpan(16, 49, 00)
            );

            // Tokyo
            yield return new TestCaseData(
                new GeoLocation(35.6762, 139.6503),
                new DateTimeOffset(2019, 11, 04, 00, 00, 00, TimeSpan.FromHours(+9)),
                new TimeSpan(16, 43, 00)
            );

            // Shenzhen
            yield return new TestCaseData(
                new GeoLocation(22.5333, 114.1333),
                new DateTimeOffset(2019, 11, 15, 00, 00, 00, TimeSpan.FromHours(+8)),
                new TimeSpan(17, 40, 00)
            );
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases_CalculateSunsetStartTime))]
        public void CalculateSunsetStartTime_Test(GeoLocation location, DateTimeOffset instant, TimeSpan expected)
        {
            // Act
            var sunsetTime = Astronomy.CalculateSunsetStartTime(location, instant);

            // Assert
            Assert.That(sunsetTime, Is.EqualTo(expected).Within(TimeSpan.FromMinutes(3)));
        }

        private static IEnumerable<TestCaseData> GetTestCases_CalculateSunsetEndTime()
        {
            // Kyiv
            yield return new TestCaseData(
                new GeoLocation(50.4547, 30.5238),
                new DateTimeOffset(2020, 04, 02, 00, 00, 00, TimeSpan.FromHours(+3)),
                new TimeSpan(20, 05, 00)
            );
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases_CalculateSunsetEndTime))]
        public void CalculateSunsetEndTime_Test(GeoLocation location, DateTimeOffset instant, TimeSpan expected)
        {
            // Act
            var sunsetTime = Astronomy.CalculateSunsetEndTime(location, instant);

            // Assert
            Assert.That(sunsetTime, Is.EqualTo(expected).Within(TimeSpan.FromMinutes(3)));
        }
    }
}