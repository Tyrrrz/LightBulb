using System.Threading.Tasks;
using LightBulb.Models;
using LightBulb.Services;
using NUnit.Framework;

namespace LightBulb.Tests.Services
{
    [TestFixture]
    public class LocationServiceTests
    {
        [Test]
        public async Task GetLocationAsync_Test()
        {
            // Arrange
            using var locationService = new LocationService();

            // Act
            var location = await locationService.GetLocationAsync();

            // Assert
            Assert.That(location, Is.Not.EqualTo(default(GeoLocation)));
        }

        [Test]
        [TestCase("Kyiv, Ukraine")]
        [TestCase("Dresden")]
        [TestCase("UK")]
        [TestCase("King's Cross, Platform 9 3/4")]
        [TestCase("Statue of Liberty")]
        public async Task GetLocationAsync_Query_Test(string query)
        {
            // Arrange
            using var locationService = new LocationService();

            // Act
            var location = await locationService.GetLocationAsync(query);

            // Assert
            Assert.That(location, Is.Not.EqualTo(default(GeoLocation)));
        }
    }
}