using System;
using System.Collections.Generic;

namespace LightBulb.Internal
{
    internal static class DisposableExtensions
    {
        public static void DisposeAll(this IEnumerable<IDisposable> disposables)
        {
            foreach (var i in disposables)
                i.Dispose();
        }
    }
}