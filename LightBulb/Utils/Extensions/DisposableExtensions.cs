using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBulb.Utils.Extensions;

internal static class DisposableExtensions
{
    public static void DisposeAll(this IEnumerable<IDisposable> disposables)
    {
        var exceptions = new List<Exception>();

        foreach (var i in disposables)
        {
            try
            {
                i.Dispose();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Any())
            throw new AggregateException(exceptions);
    }
}