using System;
using FluentAssertions;
using Xunit;

namespace LightBulb.Core.Tests;

public class CycleSpecs
{
    [Fact]
    public void I_observe_day_time_color_configuration_during_day_time()
    {
        // Arrange
        var solarTimes = new SolarTimes(new TimeOnly(07, 00), new TimeOnly(18, 00));
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
    public void I_observe_night_time_color_configuration_during_night_time()
    {
        // Arrange
        var solarTimes = new SolarTimes(new TimeOnly(07, 00), new TimeOnly(18, 00));
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
    public void I_observe_intermediate_color_configuration_during_a_transition_from_day_time_to_night_time()
    {
        // Arrange
        var solarTimes = new SolarTimes(new TimeOnly(07, 00), new TimeOnly(18, 00));
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
    public void I_observe_intermediate_color_configuration_during_a_transition_from_night_time_to_day_time()
    {
        // Arrange
        var solarTimes = new SolarTimes(new TimeOnly(07, 00), new TimeOnly(18, 00));
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
    public void I_observe_the_same_color_configuration_right_before_and_right_after_midnight()
    {
        // Arrange
        var solarTimes = new SolarTimes(new TimeOnly(07, 00), new TimeOnly(18, 00));
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
    public void I_observe_no_abrupt_changes_in_color_configuration_during_the_whole_cycle()
    {
        // Arrange
        var solarTimes = new SolarTimes(new TimeOnly(07, 20), new TimeOnly(23, 35));
        var transitionDuration = new TimeSpan(01, 30, 00);
        var transitionOffset = 0;
        var dayConfiguration = new ColorConfiguration(6600, 1);
        var nightConfiguration = new ColorConfiguration(3600, 0.85);

        // Act
        var startInstant = new DateTimeOffset(
            2019, 01, 01,
            00, 00, 00,
            TimeSpan.Zero
        );

        for (var offset = TimeSpan.Zero; offset < TimeSpan.FromDays(1); offset += TimeSpan.FromMinutes(1))
        {
            var instant1 = startInstant + offset;
            var instant2 = instant1 + TimeSpan.FromMinutes(1);

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
            var isHarshJump =
                Math.Abs(configuration1.Temperature - configuration2.Temperature) >=
                Math.Abs(dayConfiguration.Temperature - nightConfiguration.Temperature) / 2
                ||
                Math.Abs(configuration1.Brightness - configuration2.Brightness) >=
                Math.Abs(dayConfiguration.Brightness - nightConfiguration.Brightness) / 2
            ;

            Assert.False(
                isHarshJump,
                $"Detected a harsh jump in color configuration from {instant1} to {instant2}: " +
                $"{configuration1} -> {configuration2}."
            );
        }
    }
}