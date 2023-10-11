using System.Reflection;

namespace ModularSystem.Core.Reflection;

/// <summary>
/// Provides extension methods for the <see cref="Type"/> class to enhance its capabilities.
/// </summary>
public static partial class TypeAddons
{
    /// <summary>
    /// Attempts to find a method in the provided type with a given signature.
    /// </summary>
    /// <param name="type">The type in which to search for the method.</param>
    /// <param name="name">The name of the method to search for.</param>
    /// <param name="returnType">The return type of the method. Use <c>null</c> for methods that return <c>void</c>.</param>
    /// <param name="parameters">The parameter types of the method. Leave empty if the method has no parameters.</param>
    /// <returns>The <see cref="MethodInfo"/> object if a method with the specified signature is found; otherwise, <c>null</c>.</returns>
    public static MethodInfo? GetMethodInfo(this Type type, string name, Type? returnType, params Type[]? parameters)
    {
        var queryable = type.GetMethods().AsQueryable();

        queryable = queryable.Where(m => m.Name == name);

        if (returnType == null)
        {
            queryable = queryable
                .Where(m => m.ReturnType == typeof(void));
        }
        else
        {
            queryable = queryable
               .Where(m => m.ReturnType == returnType);
        }

        if (parameters == null)
        {
            queryable = queryable
                .Where(m => m.GetParameters().IsEmpty());
        }
        else
        {
            queryable = queryable
                .Where(m => m.GetParameters().All(p => parameters.Any(t => t == p.ParameterType)));
        }

        return queryable.FirstOrDefault();
    }
}
