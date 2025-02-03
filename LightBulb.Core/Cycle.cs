using System;
using LightBulb.Core.Utils.Extensions;

namespace LightBulb.Core;

public static class Cycle
{
    public static TimeOnly GetSunriseStart(
        TimeOnly sunrise,
        TimeSpan transitionDuration,
        double transitionOffset
    )
    {
        // Offset = 0; Start at (sunrise - transition)
        // Offset = 0.5; Start at (sunrise - transition/2)
        // Offset = 1; Start at (sunrise)

        var shift = transitionDuration * (1 - transitionOffset);
        return sunrise.Add(-shift);
    }

    public static TimeOnly GetSunriseEnd(
        TimeOnly sunrise,
        TimeSpan transitionDuration,
        double transitionOffset
    )
    {
        // Offset = 0; End at (sunrise)
        // Offset = 0.5; End at (sunrise + transition/2)
        // Offset = 1; End at (sunrise + transition)

        var shift = transitionDuration * transitionOffset; return sunrise.Add(shift);
    }

    public static TimeOnly GetSunsetStart(
        TimeOnly sunset,
        TimeSpan transitionDuration,
        double transitionOffset
    )
    {
        // Offset = 0; Start at (sunset)
        // Offset = 0.5; Start at (sunset - transition/2)
        // Offset = 1; Start at (sunset - transition)

        var shift = transitionDuration*transitionOffset;
        return sunset.Add( - shift);
    }

    public static TimeOnly GetSunsetEnd(
        TimeOnly sunset,
        TimeSpan transitionDuration,
        double transitionOffset
    )
    {
        // Offset = 0; End at (sunset + transition)
        // Offset = 0.5; End at (sunset + transition/2)
        // Offset = 1; End at (sunset)

                var shift = transitionDuration * (1 - transitionOffset);
        return sunset.Add(shift);
    }

    private static double Interpolate(
        SolarTimes solarTimes,
        double dayValue,
        double nightValue, TimeSpan transitionDuration,
        double transitionOffset,
        DateTimeOffset instant
    )
    {
        var sunriseStart = GetSunriseStart(
            solarTimes.Sunrise,
            transitionDuration,
            transitionOffset
        );
        var sunriseEnd = GetSunriseEnd(solarTimes.Sunrise, transitionDuration, transitionOffset);
        var sunsetStart = GetSunsetStart(solarTimes.Sunset, transitionDuration, transitionOffset);
        var sunsetEnd = GetSunsetEnd(solarTimes.Sunset, transitionDuration, transitionOffset);

        // Sunrise transition
        var prevSunriseStart = sunriseStart.PreviousBefore(instant);
        var nextSunriseEnd = sunriseEnd.NextAfter(instant);
        var isDuringSunrise = (nextSunriseEnd - prevSunriseStart).Duration() <= transitionDuration;
        if (isDuringSunrise)
        {
            var progress = (instant - prevSunriseStart) / transitionDuration;
            return dayValue + (nightValue - dayValue) * Math.Cos(progress * Math.PI / 2);
        }

        // Sunset transition
        var prevSunsetStart = sunsetStart.PreviousBefore(instant);
        var nextSunsetEnd = sunsetEnd.NextAfter(instant);
        var isDuringSunset = (nextSunsetEnd - prevSunsetStart).Duration() <= transitionDuration;
        if (isDuringSunset)
        {
            var progress = (instant - prevSunsetStart) / transitionDuration;
            return dayValue + (nightValue - dayValue) * Math.Sin(progress * Math.PI / 2);
        }

        // Day time
        if (nextSunsetEnd < nextSunriseEnd)
            return dayValue;

        // Night time
        return nightValue;
    }

    public static ColorConfiguration InterpolateConfiguration(
        SolarTimes solarTimes,
        ColorConfiguration dayConfiguration,
        ColorConfiguration nightConfiguration,
        TimeSpan transitionDuration,
        double transitionOffset,
        DateTimeOffset instant
    ) =>
        new(
            Interpolate(
                solarTimes,
                dayConfiguration.Temperature,
                nightConfiguration.Temperature,
                transitionDuration,
                transitionOffset,
                instant
            ),
            Interpolate(
                solarTimes,
                dayConfiguration.Brightness,
                nightConfiguration.Brightness,
                transitionDuration,
                transitionOffset,
                instant
            )
        );
}
