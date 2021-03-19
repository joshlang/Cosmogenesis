using System;
using System.Collections.Generic;
using System.Linq;

namespace Cosmogenesis.Core
{
    static class TypeExtensions
    {
        public static IEnumerable<Type> GetGenericInterfaces(this Type type, Type genericInterfaceType) =>
            type is null
            ? throw new ArgumentNullException(nameof(type))
            : genericInterfaceType is null
            ? throw new ArgumentNullException(nameof(genericInterfaceType))
            : !genericInterfaceType.IsInterface || !genericInterfaceType.IsGenericType
            ? throw new InvalidOperationException("Parameter must be a generic interface type")
            : type
                .GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == genericInterfaceType);
    }
}
