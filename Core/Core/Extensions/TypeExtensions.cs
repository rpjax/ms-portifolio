using System.Reflection;

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
    /// Generic version of IsSubtypeOf, facilitating checks against a specific parent type.
    /// </summary>
    /// <param name="childType">The type that might be a subtype.</param>
    /// <returns>True if the childType is a subtype of T, otherwise false.</returns>
    public static bool IsSubtypeOf<T>(this Type childType)
    {
        return IsSubtypeOf(childType, typeof(T));
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

    public static int? InheritanceDistance(this Type baseType, Type derivedType)
    {
        int distance = 0;

        Type? targetType = derivedType;

        while (targetType != null && targetType != baseType)
        {
            targetType = targetType.BaseType;
            distance++;
        }

        if (derivedType == baseType)
        {
            return distance;
        }

        return null; // means derivedType is not derived from baseType
    }

    public static bool IsNullable(this Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }

    /// <summary>
    /// Retrieves a specific method from a type based on the given method name, parameter types, and return type.
    /// </summary>
    /// <param name="self">The type from which to retrieve the method.</param>
    /// <param name="name">The name of the method to retrieve.</param>
    /// <param name="parameters">An array of types representing the parameters of the method.</param>
    /// <param name="outputType">The return type of the method.</param>
    /// <returns>
    /// The <see cref="MethodInfo"/> object representing the method that matches the specified criteria;
    /// returns null if no such method is found.
    /// </returns>
    /// <exception cref="AmbiguousMatchException">
    /// Thrown when multiple methods match the specified criteria, making the method call ambiguous.
    /// </exception>
    /// <remarks>
    /// This method is useful for retrieving methods in cases where multiple overloads may exist,
    /// and a specific method needs to be obtained based on its signature.
    /// </remarks>
    public static MethodInfo? GetMethod(this Type self, string name, Type[] parameters, Type outputType)
    {
        var methods = self
            .GetMethods()
            .Where(x => x.Name == name)
            .Where(x => x.ReturnType == outputType)
            .Where(x => parameters.All(paramType => x.GetParameters().Any(methodParam => methodParam.ParameterType == paramType)))
            .ToArray();

        if (methods.Length == 0)
        {
            return null;
        }
        if (methods.Length > 1)
        {
            throw new AmbiguousMatchException($"Multiple methods found matching the name '{name}', parameter types, and return type. Please specify more distinct criteria.");
        }

        return methods[0];
    }

    public static Type? TryGetGenericTypeDefinition(this Type type)
    {
        if (!type.IsGenericType)
        {
            return null;
        }

        return type.GetGenericTypeDefinition();
    }

}
