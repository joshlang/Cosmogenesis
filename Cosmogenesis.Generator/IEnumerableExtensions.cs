using System.Collections.Generic;
using System.Linq;

namespace Cosmogenesis.Generator
{
    static class IEnumerableExtensions
    {
        /// <summary>
        /// Excludes all null elements and returns an enumeration of non-nullable items.
        /// Equivalent to .Where(x=>x!=null).Select(x=>x!)
        /// </summary>
        public static IEnumerable<T> ExcludeNull<T>(this IEnumerable<T?> items) where T : class => items.Where(x => x != null)!;
    }
}
