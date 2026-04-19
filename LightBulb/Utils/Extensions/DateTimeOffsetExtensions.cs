using System;

namespace LightBulb.Utils.Extensions;

internal static class DateTimeOffsetExtensions
{
    extension(DateTimeOffset value)
    {
        public DateTimeOffset StepTo(DateTimeOffset target, TimeSpan step)
        {
            if (target >= value)
            {
                var result = value + step.Duration();
                return result <= target ? result : target;
            }
            else
            {
                var result = value - step.Duration();
                return result >= target ? result : target;
            }
        }
    }
}
