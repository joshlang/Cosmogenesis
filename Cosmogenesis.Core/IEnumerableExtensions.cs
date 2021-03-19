using System;
using System.Collections.Generic;
using System.Linq;

namespace Cosmogenesis.Core
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Splits the items into segments with a max size.
        /// Unless the returned segment is enumerated, this will return an infinite sequence.
        /// If the segment is partially enumerated, the next segment will pick up from where you left off (no elements will be missed).
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Segment<T>(this IEnumerable<T>? items, int maxItemsPerSegment)
        {
            if (maxItemsPerSegment <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxItemsPerSegment));
            }
            if (items is null)
            {
                yield break;
            }

            var done = false;
            IEnumerable<T> Next(IEnumerator<T> enumerator)
            {
                for (var x = 0; x < maxItemsPerSegment; ++x)
                {
                    yield return enumerator.Current;
                    if (!enumerator.MoveNext())
                    {
                        done = true;
                        yield break;
                    }
                }
            }
            using var e = items.GetEnumerator();
            if (!e.MoveNext())
            {
                yield break;
            }
            while (!done)
            {
                yield return Next(e);
            }
        }

        /// <summary>
        /// Splits the items into segments with a max size.
        /// The items are materialized into a list.
        /// </summary>
        public static IEnumerable<List<T>> ReuseableSegment<T>(this IEnumerable<T>? items, int maxItemsPerSegment)
        {
            const int MaxFirstPreallocation = 1024;
            const int MaxPreallocation = 1024 * 1024;

            if (maxItemsPerSegment <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxItemsPerSegment));
            }
            if (items is null)
            {
                yield break;
            }
            var next = new List<T>(maxItemsPerSegment > MaxFirstPreallocation ? MaxFirstPreallocation : maxItemsPerSegment);
            foreach (var i in items)
            {
                next.Add(i);
                if (next.Count == maxItemsPerSegment)
                {
                    yield return next;
                    next = new List<T>(maxItemsPerSegment > MaxPreallocation ? MaxPreallocation : maxItemsPerSegment);
                }
            }
            if (next.Count > 0)
            {
                yield return next;
            }
        }

        /// <summary>
        /// This can safely be called on an IEnumerable[T] which is null, and it will convert it into Enumerable.Empty[T]() if it's null.
        /// </summary>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? items) => items ?? Enumerable.Empty<T>();

        /// <summary>
        /// Excludes all null elements and returns an enumeration of non-nullable items.
        /// Equivalent to .Where(x=>x!=null).Select(x=>x!)
        /// </summary>
        public static IEnumerable<T> ExcludeNull<T>(this IEnumerable<T?> items) where T : class => items.Where(x => x != null)!;

        /// <summary>
        /// This converts an IEnumerable[T] to a IEnumerable[T?]
        /// </summary>
        public static IEnumerable<T?> Nullable<T>(this IEnumerable<T> items) where T : class => items.Select(x => x ?? null);
    }
}
