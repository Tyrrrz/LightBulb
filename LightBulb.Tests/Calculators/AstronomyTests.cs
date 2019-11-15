using System;
using System.Collections.Generic;
using LightBulb.Calculators;
using LightBulb.Models;
using NUnit.Framework;

namespace LightBulb.Tests.Calculators
{
    [TestFixture]
    public class AstronomyTests
    {
        private static IEnumerable<TestCaseData> GetTestCases_CalculateSunriseTime()
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
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases_CalculateSunriseTime))]
        public void CalculateSunriseTime_Test(GeoLocation location, DateTimeOffset instant, TimeSpan expectedSunriseTime)
        {
            // Act
            var sunriseTime = Astronomy.CalculateSunriseTime(location, instant);

            // Assert
            Assert.That(sunriseTime, Is.EqualTo(expectedSunriseTime).Within(TimeSpan.FromMinutes(3)));
        }

        private static IEnumerable<TestCaseData> GetTestCases_CalculateSunsetTime()
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
        [TestCaseSource(nameof(GetTestCases_CalculateSunsetTime))]
        public void CalculateSunsetTime_Test(GeoLocation location, DateTimeOffset instant, TimeSpan expectedSunsetTime)
        {
            // Act
            var sunsetTime = Astronomy.CalculateSunsetTime(location, instant);

            // Assert
            Assert.That(sunsetTime, Is.EqualTo(expectedSunsetTime).Within(TimeSpan.FromMinutes(3)));
        }
    }
}