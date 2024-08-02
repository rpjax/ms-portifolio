using Aidan.Webql.Analysis.Semantics;
using System.Collections;

namespace Aidan.Webql.Synthesis.Compilation.LINQ.Extensions;

public static class ArgumentSemanticsExtensions
{
    public static bool IsQueryable(this ArgumentSemantic semantics)
    {
        return
            typeof(IEnumerable).IsAssignableFrom(semantics.Type)
            || semantics.Type.GetInterfaces().Any(i =>
               i.IsGenericType &&
               i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static bool IsNotQueryable(this ArgumentSemantic semantics)
    {
        return !IsQueryable(semantics);
    }

    public static bool IsEnumerable(this ArgumentSemantic semantics)
    {
        var type = semantics.Type;

        if (type == typeof(string))
        {
            return false;
        }

        // Verifica se o tipo é genérico e implementa IEnumerable<T>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return true;
        }

        // Verifica se algum dos tipos de interface implementados é um IEnumerable<T>
        foreach (Type interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsNotEnumerable(this ArgumentSemantic semantics)
    {
        return !IsEnumerable(semantics);
    }

    public static Type? TryGetElementType(this ArgumentSemantic semantics)
    {
        var type = semantics.Type;

        if (IsNotQueryable(semantics))
        {
            return null;
        }

        if (type.IsArray)
        {
            return type.GetElementType();
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

    public static Type GetElementType(this ArgumentSemantic semantics, TranslationContext context)
    {
        var type = TryGetElementType(semantics);

        if (type == null)
        {
            throw new Exception("The current context does not represent a queryable type or the queryable type is undefined. Ensure that the context is correctly initialized and represents a valid queryable type. This error may indicate a misalignment between the expected and actual types within the context.");
        }

        return type;
    }

    public static LinqSourceType GetLinqSourceType(this ArgumentSemantic semantics, TranslationContext context)
    {
        if (IsQueryable(semantics))
        {
            return LinqSourceType.Queryable;
        }
        if (IsEnumerable(semantics))
        {
            return LinqSourceType.Enumerable;
        }

        var message = $"The type '{semantics.Type.FullName}' is not supported for LINQ operations because it does not implement either IQueryable<T> or IEnumerable<T>.";

        throw new Exception(message);
    }

}
