using System.Collections.Generic;

namespace LightBulb.WindowsApi.Internal
{
    internal static class CollectionExtensions
    {
        public static void AddIfNotNull<T>(this List<T> list, T? item) where T : class
        {
            if (item != null)
                list.Add(item);
        }
    }
}