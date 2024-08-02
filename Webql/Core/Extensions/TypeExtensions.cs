﻿using Aidan.Core.Linq;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Webql.Core.Extensions;

public static class TypeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsQueryable(this Type type)
    {
        if(type == typeof(string))
        {
            return false;
        }

        return false
            || typeof(IEnumerable).IsAssignableFrom(type)
            || type.GetInterfaces().Any(i =>
               i.IsGenericType &&
               i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsyncQueryable(this Type type)
    {
        return false
            || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IAsyncQueryable<>)
            || type.GetInterfaces().Any(i =>
               i.IsGenericType &&
               i.GetGenericTypeDefinition() == typeof(IAsyncQueryable<>));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotQueryable(this Type type)
    {
        return !IsQueryable(type);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotAsyncQueryable(this Type type)
    {
        return !IsAsyncQueryable(type);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type GetQueryableElementType(this Type type)
    {
        var elementType = TryGetQueryableElementType(type);

        if (elementType == null)
        {
            throw new InvalidOperationException("The type does not have an element type.");
        }

        return elementType;
    }
}
