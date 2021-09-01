using System;
using FluentAssertions;
using Xunit;

namespace LightBulb.Core.Tests
{
    public class CycleSpecs
    {
        [Fact]
        public void Color_configuration_is_set_to_day_configuration_during_day_time()
        {
            // Arrange
            var solarTimes = new SolarTimes(new TimeOfDay(07, 00), new TimeOfDay(18, 00));
            var transitionDuration = new TimeSpan(01, 30, 00);
            var transitionOffset = 0;
            var dayConfiguration = new ColorConfiguration(6600, 1);
            var nightConfiguration = new ColorConfiguration(3600, 0.85);

            var instant = new DateTimeOffset(
                2019, 01, 01,
                14, 00, 00,
                TimeSpan.Zero
            );

            // Act
            var configuration = Cycle.GetInterpolatedConfiguration(
                solarTimes,
                dayConfiguration,
                nightConfiguration,
                transitionDuration,
                transitionOffset,
                instant
            );

            // Assert
            configuration.Should().Be(dayConfiguration);
        }

        [Fact]
        public void Color_configuration_is_set_to_night_configuration_during_night_time()
        {
            // Arrange
            var solarTimes = new SolarTimes(new TimeOfDay(07, 00), new TimeOfDay(18, 00));
            var transitionDuration = new TimeSpan(01, 30, 00);
            var transitionOffset = 0;
            var dayConfiguration = new ColorConfiguration(6600, 1);
            var nightConfiguration = new ColorConfiguration(3600, 0.85);

            var instant = new DateTimeOffset(
                2019, 01, 01,
                02, 00, 00,
                TimeSpan.Zero
            );

            // Act
            var configuration = Cycle.GetInterpolatedConfiguration(
                solarTimes,
                dayConfiguration,
                nightConfiguration,
                transitionDuration,
                transitionOffset,
                instant
            );

            // Assert
            configuration.Should().Be(nightConfiguration);
        }

        [Fact]
        public void Color_configuration_is_set_to_intermediate_value_during_transition_to_night_time()
        {
            // Arrange
            var solarTimes = new SolarTimes(new TimeOfDay(07, 00), new TimeOfDay(18, 00));
            var transitionDuration = new TimeSpan(01, 30, 00);
            var transitionOffset = 0;
            var dayConfiguration = new ColorConfiguration(6600, 1);
            var nightConfiguration = new ColorConfiguration(3600, 0.85);

            var instant = new DateTimeOffset(
                2019, 01, 01,
                18, 30, 00,
                TimeSpan.Zero
            );

            // Act
            var configuration = Cycle.GetInterpolatedConfiguration(
                solarTimes,
                dayConfiguration,
                nightConfiguration,
                transitionDuration,
                transitionOffset,
                instant
            );

            // Assert
            configuration.Temperature.Should().BeLessThan(dayConfiguration.Temperature);
            configuration.Temperature.Should().BeGreaterThan(nightConfiguration.Temperature);
            configuration.Brightness.Should().BeLessThan(dayConfiguration.Brightness);
            configuration.Brightness.Should().BeGreaterThan(nightConfiguration.Brightness);
        }

        [Fact]
        public void Color_configuration_is_set_to_intermediate_value_during_transition_to_day_time()
        {
            // Arrange
            var solarTimes = new SolarTimes(new TimeOfDay(07, 00), new TimeOfDay(18, 00));
            var transitionDuration = new TimeSpan(01, 30, 00);
            var transitionOffset = 0;
            var dayConfiguration = new ColorConfiguration(6600, 1);
            var nightConfiguration = new ColorConfiguration(3600, 0.85);

            var instant = new DateTimeOffset(
                2019, 01, 01,
                18, 30, 00,
                TimeSpan.Zero
            );

            // Act
            var configuration = Cycle.GetInterpolatedConfiguration(
                solarTimes,
                dayConfiguration,
                nightConfiguration,
                transitionDuration,
                transitionOffset,
                instant
            );

            // Assert
            configuration.Temperature.Should().BeLessThan(dayConfiguration.Temperature);
            configuration.Temperature.Should().BeGreaterThan(nightConfiguration.Temperature);
            configuration.Brightness.Should().BeLessThan(dayConfiguration.Brightness);
            configuration.Brightness.Should().BeGreaterThan(nightConfiguration.Brightness);
        }

        [Fact]
        public void Color_configuration_is_the_same_at_the_cycle_boundaries()
        {
            // Arrange
            var solarTimes = new SolarTimes(new TimeOfDay(07, 00), new TimeOfDay(18, 00));
            var transitionDuration = new TimeSpan(01, 30, 00);
            var transitionOffset = 0;
            var dayConfiguration = new ColorConfiguration(6600, 1);
            var nightConfiguration = new ColorConfiguration(3600, 0.85);

            var instant1 = new DateTimeOffset(
                2019, 01, 01,
                00, 00, 00,
                TimeSpan.Zero
            );

            var instant2 = new DateTimeOffset(
                2019, 01, 01,
                23, 59, 59,
                TimeSpan.Zero
            );

            // Act
            var configuration1 = Cycle.GetInterpolatedConfiguration(
                solarTimes,
                dayConfiguration,
                nightConfiguration,
                transitionDuration,
                transitionOffset,
                instant1
            );

            var configuration2 = Cycle.GetInterpolatedConfiguration(
                solarTimes,
                dayConfiguration,
                nightConfiguration,
                transitionDuration,
                transitionOffset,
                instant2
            );

            // Assert
            configuration1.Should().Be(configuration2);
        }

        [Fact]
        public void Color_configuration_does_not_change_abruptly_throughout_the_whole_cycle()
        {
            // Arrange
            var solarTimes = new SolarTimes(new TimeOfDay(07, 20), new TimeOfDay(23, 35));
            var transitionDuration = new TimeSpan(01, 30, 00);
            var transitionOffset = 0;
            var dayConfiguration = new ColorConfiguration(6600, 1);
            var nightConfiguration = new ColorConfiguration(3600, 0.85);

            // Act
            var lastInstantTime = default(TimeSpan?);
            var lastConfiguration = default(ColorConfiguration?);

            for (var instantTime = TimeSpan.Zero; instantTime < TimeSpan.FromDays(1); instantTime += TimeSpan.FromMinutes(1))
            {
                var instant = new DateTimeOffset(
                    2019, 01, 01,
                    instantTime.Hours, instantTime.Minutes, instantTime.Seconds,
                    TimeSpan.Zero
                );

                var configuration = Cycle.GetInterpolatedConfiguration(
                    solarTimes,
                    dayConfiguration,
                    nightConfiguration,
                    transitionDuration,
                    transitionOffset,
                    instant
                );

                // Assert
                var isHarshJump =
                    lastInstantTime is not null &&
                    lastConfiguration is not null &&
                    (
                        Math.Abs(configuration.Temperature - lastConfiguration.Value.Temperature) >=
                        Math.Abs(dayConfiguration.Temperature - nightConfiguration.Temperature) / 2
                        ||
                        Math.Abs(configuration.Brightness - lastConfiguration.Value.Brightness) >=
                        Math.Abs(dayConfiguration.Brightness - nightConfiguration.Brightness) / 2
                    );

                Assert.False(
                    isHarshJump,
                    $"Detected harsh jump in color configuration from {lastInstantTime} to {instantTime}: " +
                    $"{lastConfiguration} -> {configuration}."
                );

                lastInstantTime = instantTime;
                lastConfiguration = configuration;
            }
        }
    }
}