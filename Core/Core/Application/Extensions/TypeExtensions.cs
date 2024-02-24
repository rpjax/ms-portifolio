using System.Collections;

namespace ModularSystem.Core;

/// <summary>
/// Provides extension methods for the .NET System.Type class, offering useful functionalities for working with types and type hierarchies at runtime.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Checks whether a child type is a subtype of a parent type.
    /// </summary>
    /// <param name="childType">The type that might be a subtype.</param>
    /// <param name="parentType">The type that might be a supertype.</param>
    /// <returns>True if the childType is a subtype of parentType, otherwise false.</returns>
    /// <remarks>
    /// Considers generic types, ensuring that a List of int is not considered equal to a List of string, for example.
    /// </remarks>
    public static bool IsSubtypeOf(this Type childType, Type parentType)
    {
        Type? iteratorType = childType;

        while (iteratorType != null && iteratorType != typeof(object))
        {
            Type type;

            //*
            // this ensures that List<int> is not equal to List<string>.
            //*
            if (iteratorType.IsGenericType && parentType.IsGenericType)
            {
                type = iteratorType.GetGenericTypeDefinition();
            }
            else
            {
                type = iteratorType;
            }

            if (parentType == type)
            {
                return true;
            }

            iteratorType = iteratorType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified type implements the specified interface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="interfaceType">The interface to check against.</param>
    /// <returns>True if the type implements the interface, otherwise false.</returns>
    /// <remarks>
    /// Works with generic interfaces.
    /// </remarks>
    public static bool ImplementsInterface(this Type type, Type interfaceType)
    {
        foreach (var implementedInterface in type.GetInterfaces())
        {
            if (implementedInterface == interfaceType ||
                (interfaceType.IsGenericType && implementedInterface.IsGenericType &&
                implementedInterface.GetGenericTypeDefinition() == interfaceType))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the source type is an instance of a generic type matching the target type.
    /// </summary>
    /// <param name="source">The source type to check.</param>
    /// <param name="target">The target generic type.</param>
    /// <returns>True if the source type is an instance of the target generic type, otherwise false.</returns>
    public static bool IsInstanceOfGenericType(this Type source, Type target)
    {
        Type? _type = source;

        while (_type != null && _type != typeof(object))
        {
            if (_type.IsGenericType && _type.GetGenericTypeDefinition() == target)
                return true;

            _type = _type.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Generic version of IsInstanceOfGenericType.
    /// </summary>
    /// <param name="source">The source type to check.</param>
    /// <returns>True if the source type is an instance of T, otherwise false.</returns>
    public static bool IsInstanceOfGenericType<T>(this Type source)
    {
        return source.IsInstanceOfGenericType(typeof(T));
    }

    public static bool IsNullable(this Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }

    public static Type? TryGetGenericTypeDefinition(this Type type)
    {
        if (!type.IsGenericType)
        {
            return null;
        }

        return type.GetGenericTypeDefinition();
    }

    /// <summary>
    /// Retrieves the fully qualified name of a type, with special handling for generic types.
    /// </summary>
    /// <param name="type">The type for which the fully qualified name is being retrieved.</param>
    /// <returns>The fully qualified name of the type.</returns>
    /// <exception cref="ArgumentException">Thrown when the full name of the type is null.</exception>
    public static string GetQualifiedAssemblyName(this Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var genericTypeName = genericTypeDefinition.AssemblyQualifiedName;

            if (genericTypeName == null)
            {
                throw new ArgumentException($"Missing argument: {nameof(Type)}.{nameof(Type.AssemblyQualifiedName)}");
            }

            return genericTypeName;
        }

        if (type.AssemblyQualifiedName == null)
        {
            throw new ArgumentException($"Missing argument: {nameof(Type)}.{nameof(Type.AssemblyQualifiedName)}");
        }

        return type.AssemblyQualifiedName;
    }

    public static bool IsEnumerable(this Type type)
    {
        return
            typeof(IEnumerable).IsAssignableFrom(type)
            || type.GetInterfaces().Any(i =>
               i.IsGenericType &&
               i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static Type? TryGetEnumerableType(this Type type)
    {
        if (type.IsEnumerable())
        {
            if (type!.IsArray)
            {
                return type.GetElementType();
            }
            else
            {
                return type.GetGenericArguments().FirstOrDefault();
            }
        }

        return null;
    }

}

/// <summary>
/// Provides methods for comparing type instances, including support for generic type comparisons.
/// </summary>
public static class TypeComparer
{
    /// <summary>
    /// Compares two types, considering generic type definitions.
    /// </summary>
    /// <param name="type1">The first type to compare.</param>
    /// <param name="type2">The second type to compare.</param>
    /// <returns>
    /// True if both types are the same or if they are generic types with the same generic type definition;
    /// otherwise, false.
    /// </returns>
    /// <remarks>
    /// This method is particularly useful for comparing types where the specific type arguments of generic types
    /// are not of interest, but rather the generic type definitions themselves.
    /// </remarks>
    public static bool GenericCompare(Type type1, Type type2)
    {
        if (type1.IsGenericType && type2.IsGenericType)
        {           
            var genericType1 = type1.GetGenericTypeDefinition();
            var genericType2 = type2.GetGenericTypeDefinition();

            var genericArgs1 = type1.GetGenericArguments();
            var genericArgs2 = type2.GetGenericArguments();

            if(genericType1 != genericType2)
            {
                return false;
            }
            if (genericArgs1.Length != genericArgs2.Length)
            {
                return false;
            }

            for (int i = 0; i < genericArgs1.Length; i++)
            {
                if(genericArgs1[i].IsGenericParameter || genericArgs2[i].IsGenericParameter)
                {
                    continue;
                }

                if (!GenericCompare(genericArgs1[i], genericArgs2[i]))
                {
                    return false;
                }
            }

            return true;     
        }

        return type1 == type2;
    }
}
