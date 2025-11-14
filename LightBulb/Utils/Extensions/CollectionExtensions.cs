using System.Collections.Generic;

namespace LightBulb.Utils.Extensions;

internal static class CollectionExtensions
{
    extension<T>(IEnumerable<T?> source)
        where T : class
    {
        public IEnumerable<T> WhereNotNull()
        {
            foreach (var item in source)
            {
                if (item is not null)
                    yield return item;
            }
        }
    }
}
