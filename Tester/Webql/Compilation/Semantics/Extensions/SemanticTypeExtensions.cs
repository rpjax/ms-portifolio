using ModularSystem.Core.Linq;
using System.Collections;

namespace Webql.Semantics.Extensions;

public static class SemanticTypeExtensions
{
    public static bool IsQueryable(this Type type)
    {
        return
            typeof(IEnumerable).IsAssignableFrom(type)
            || type.GetInterfaces().Any(i =>
               i.IsGenericType &&
               i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static bool IsNotQueryable(this Type type)
    {
        return !IsQueryable(type);
    }

    public static Type? TryGetQueryableElementType(this Type type)
    {
        if (IsNotQueryable(type))
        {
            return null;
        }

        if (type.IsArray)
        {
            return type.GetElementType();
        }

        var asyncQueryableInterface = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IAsyncQueryable<>))
            .FirstOrDefault();

        if (asyncQueryableInterface != null)
        {
            return asyncQueryableInterface.GetGenericArguments()[0];
        }

        var queryableInterface = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IQueryable<>))
            .FirstOrDefault();

        if (queryableInterface != null)
        {
            return queryableInterface.GetGenericArguments()[0];
        }

        var enumerableInterface = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .FirstOrDefault();

        if (enumerableInterface != null)
        {
            return enumerableInterface.GetGenericArguments()[0];
        }

        return null;
    }

    public static Type GetQueryableElementType(this Type type)
    {
        var elementType = TryGetQueryableElementType(type);

        if (elementType == null)
        {
            throw new InvalidOperationException("The type does not have an element type.");
        }

        return type;
    }
}

