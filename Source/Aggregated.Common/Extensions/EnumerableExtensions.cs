using System.Collections.Generic;
using System.Linq;

namespace Aggregated.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T source)
        {
            yield return source;
        }

        public static ICollection<T> AsCollection<T>(this IEnumerable<T> source)
        {
            return source as ICollection<T> ?? source.ToList();
        }
    }
}
