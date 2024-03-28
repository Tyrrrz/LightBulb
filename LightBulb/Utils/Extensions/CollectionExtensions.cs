using System.Collections.Generic;

namespace LightBulb.Utils.Extensions;

internal static class CollectionExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
    {
        foreach (var item in source)
        {
            if (item is not null)
                yield return item;
        }
    }
}