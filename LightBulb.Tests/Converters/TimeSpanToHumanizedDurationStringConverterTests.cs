using System;
using System.Collections.Generic;
using System.Globalization;
using LightBulb.Converters;
using NUnit.Framework;

namespace LightBulb.Tests.Converters
{
    [TestFixture]
    public class TimeSpanToHumanizedDurationStringConverterTests
    {
        private static IEnumerable<TestCaseData> GetTestCases_Convert()
        {
            yield return new TestCaseData(
                TimeSpan.FromHours(1),
                "1 hour",
                null
            );

            yield return new TestCaseData(
                TimeSpan.FromHours(2),
                "2 hours",
                null
            );

            yield return new TestCaseData(
                TimeSpan.FromHours(1.5),
                "1 hour 30 minutes",
                null
            );

            yield return new TestCaseData(
                TimeSpan.FromHours(4.5),
                "4 hours 30 minutes",
                null
            );

            yield return new TestCaseData(
                TimeSpan.FromHours(1) + TimeSpan.FromMinutes(1),
                "1 hour 1 minute",
                null
            );

            yield return new TestCaseData(
                TimeSpan.FromHours(2) + TimeSpan.FromMinutes(1),
                "2 hours 1 minute",
                null
            );

            yield return new TestCaseData(
                TimeSpan.FromSeconds(10),
                "10 seconds",
                null
            );

            yield return new TestCaseData(
                TimeSpan.FromSeconds(1),
                "1 second",
                null
            );

            yield return new TestCaseData(
                TimeSpan.FromSeconds(0),
                "0 seconds",
                null
            );

            yield return new TestCaseData(
                TimeSpan.Zero,
                "0 seconds",
                null
            );
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases_Convert))]
        public void Convert_Test(TimeSpan input, string expectedOutput, CultureInfo cultureInfo)
        {
            // Arrange
            var converter = new TimeSpanToHumanizedDurationStringConverter();

            // Act
            var output = converter.Convert(input, expectedOutput.GetType(), null, cultureInfo);

            // Assert
            Assert.That(output, Is.EqualTo(expectedOutput));
        }
    }
}