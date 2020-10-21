using System;
using System.Collections.Generic;

namespace LightBulb.Internal.Extensions
{
    internal static class CollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> callback)
        {
            foreach (var i in source)
                callback(i);
        }
    }
}