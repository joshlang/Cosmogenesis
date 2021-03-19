using System.Collections.Generic;
using System.Linq;

namespace Cosmogenesis.Core
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Changes the query type to `T`.
        /// The Cosmos query provider does not support the .Cast() method, so use this instead.
        /// The types do not need to actually be compatible; instead, you're simply choosing the shape of the data as you want to work with it.
        /// </summary>
        public static IQueryable<T> ForceCast<T>(this IQueryable<object> query) => query.Select(x => (T)x);

        /// <summary>
        /// Changes the query type to `DbDoc`.
        /// The serializer will create the derived types based on the `Type` field, or will throw an exception if `Type` is missing or not recognized.
        /// </summary>
        public static IQueryable<DbDoc> AsKnownDocument(this IQueryable<object> query) => query.ForceCast<DbDoc>();

        /// <summary>
        /// Changes the query type to `IDictionary[string, object]`.
        /// While building the query, you can access any dictionary key, existing or not, and treat it as any type.
        /// The serializer will return a regular IDictionary (where you should check if keys exist). 
        /// </summary>
        public static IQueryable<IDictionary<string, object>> AsDynamic(this IQueryable<object> query) => query.ForceCast<IDictionary<string, object>>();
    }
}
