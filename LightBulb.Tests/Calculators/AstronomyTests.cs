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
        private static IEnumerable<TestCaseData> GetTestCases_CalculateSunrise()
        {
            yield return new TestCaseData(
                new GeoLocation(40.7128, -74.0060),
                new DateTimeOffset(2019, 10, 08, 00, 00, 00, TimeSpan.FromHours(-4)),
                new TimeSpan(07, 01, 00)
            );

            yield return new TestCaseData(
                new GeoLocation(40.7128, -74.0060),
                new DateTimeOffset(2019, 10, 08, 13, 37, 59, TimeSpan.FromHours(-4)),
                new TimeSpan(07, 01, 00)
            );

            yield return new TestCaseData(
                new GeoLocation(55.6761, 12.5683),
                new DateTimeOffset(2019, 10, 08, 00, 00, 00, TimeSpan.FromHours(+2)),
                new TimeSpan(07, 27, 00)
            );

            yield return new TestCaseData(
                new GeoLocation(55.6761, 12.5683),
                new DateTimeOffset(2019, 10, 08, 05, 41, 01, TimeSpan.FromHours(+2)),
                new TimeSpan(07, 27, 00)
            );
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases_CalculateSunrise))]
        public void CalculateSunrise_Test(GeoLocation location, DateTimeOffset instant, TimeSpan expectedSunriseTime)
        {
            // Act
            var sunriseTime = Astronomy.CalculateSunrise(location, instant).TimeOfDay;

            // Assert
            Assert.That(sunriseTime, Is.EqualTo(expectedSunriseTime).Within(TimeSpan.FromMinutes(3)));
        }

        private static IEnumerable<TestCaseData> GetTestCases_CalculateSunset()
        {
            yield return new TestCaseData(
                new GeoLocation(40.7128, -74.0060),
                new DateTimeOffset(2019, 10, 08, 00, 00, 00, TimeSpan.FromHours(-4)),
                new TimeSpan(18, 27, 00)
            );

            yield return new TestCaseData(
                new GeoLocation(40.7128, -74.0060),
                new DateTimeOffset(2019, 10, 08, 13, 37, 59, TimeSpan.FromHours(-4)),
                new TimeSpan(18, 27, 00)
            );

            yield return new TestCaseData(
                new GeoLocation(35.6762, 139.6503),
                new DateTimeOffset(2019, 10, 08, 00, 00, 00, TimeSpan.FromHours(+9)),
                new TimeSpan(17, 15, 00)
            );

            yield return new TestCaseData(
                new GeoLocation(35.6762, 139.6503),
                new DateTimeOffset(2019, 10, 08, 23, 59, 59, TimeSpan.FromHours(+9)),
                new TimeSpan(17, 15, 00)
            );
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases_CalculateSunset))]
        public void CalculateSunset_Test(GeoLocation location, DateTimeOffset instant, TimeSpan expectedSunsetTime)
        {
            // Act
            var sunsetTime = Astronomy.CalculateSunset(location, instant).TimeOfDay;

            // Assert
            Assert.That(sunsetTime, Is.EqualTo(expectedSunsetTime).Within(TimeSpan.FromMinutes(3)));
        }
    }
}