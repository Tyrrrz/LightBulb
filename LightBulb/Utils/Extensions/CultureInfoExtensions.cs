using System.Collections.Generic;
using System.Globalization;

namespace LightBulb.Utils.Extensions;

internal static class CultureInfoExtensions
{
    extension(CultureInfo culture)
    {
        public IEnumerable<CultureInfo> GetSelfAndParents()
        {
            for (var c = culture; c.Name != string.Empty; c = c.Parent)
                yield return c;
        }
    }
}
