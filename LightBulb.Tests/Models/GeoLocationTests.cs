using System.Collections.Generic;
using LightBulb.Models;
using NUnit.Framework;

namespace LightBulb.Tests.Models
{
    [TestFixture]
    public class GeoLocationTests
    {
        private static IEnumerable<TestCaseData> GetTestCases_TryParse()
        {
            // Valid
            yield return new TestCaseData("41.25 -120.9762", true, new GeoLocation(41.25, -120.9762));
            yield return new TestCaseData("41.25, -120.9762", true, new GeoLocation(41.25, -120.9762));
            yield return new TestCaseData("41.25,-120.9762", true, new GeoLocation(41.25, -120.9762));
            yield return new TestCaseData("-41.25, -120.9762", true, new GeoLocation(-41.25, -120.9762));
            yield return new TestCaseData("41.25, 120.9762", true, new GeoLocation(41.25, 120.9762));
            yield return new TestCaseData("41.25°N, 120.9762°W", true, new GeoLocation(41.25, -120.9762));
            yield return new TestCaseData("41.25N 120.9762W", true, new GeoLocation(41.25, -120.9762));
            yield return new TestCaseData("41.25N, 120.9762W", true, new GeoLocation(41.25, -120.9762));
            yield return new TestCaseData("41.25 N, 120.9762 W", true, new GeoLocation(41.25, -120.9762));
            yield return new TestCaseData("41.25 S, 120.9762 W", true, new GeoLocation(-41.25, -120.9762));
            yield return new TestCaseData("41.25 S, 120.9762 E", true, new GeoLocation(-41.25, 120.9762));
            yield return new TestCaseData("41, 120", true, new GeoLocation(41, 120));
            yield return new TestCaseData("-41, -120", true, new GeoLocation(-41, -120));
            yield return new TestCaseData("41 N, 120 E", true, new GeoLocation(41, 120));
            yield return new TestCaseData("41 N, 120 W", true, new GeoLocation(41, -120));

            // Invalid
            yield return new TestCaseData("41.25; -120.9762", false, default(GeoLocation));
            yield return new TestCaseData("-41.25 S, 120.9762 E", false, default(GeoLocation));
            yield return new TestCaseData("41.25", false, default(GeoLocation));
            yield return new TestCaseData("", false, default(GeoLocation));
            yield return new TestCaseData(null, false, default(GeoLocation));
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases_TryParse))]
        public void TryParse_Test(string value, bool isValid, GeoLocation expectedResult = default)
        {
            // Act
            var success = GeoLocation.TryParse(value, out var result);

            // Assert
            Assert.That(success, Is.EqualTo(isValid), "Is valid");
            Assert.That(result, Is.EqualTo(expectedResult), "Result value");
        }
    }
}