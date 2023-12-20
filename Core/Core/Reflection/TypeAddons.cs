using System.Reflection;

namespace ModularSystem.Core.Reflection;

/// <summary>
/// Provides extension methods for the <see cref="Type"/> class to enhance its capabilities.
/// </summary>
public static partial class TypeAddons
{
    /// <summary>
    /// Checks if a given type is a dynamically created anonymous type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsAnonymous(this Type type)
    {
        return
            type.FullName != null
            && type.FullName.StartsWith(TypeCreator.AnonymousTypePrefix);
    }

    /// <summary>
    /// Attempts to find a method in the provided type with a given signature.
    /// </summary>
    /// <param name="type">The type in which to search for the method.</param>
    /// <param name="name">The name of the method to search for.</param>
    /// <param name="returnType">The return type of the method. Use <c>null</c> for methods that return <c>void</c>.</param>
    /// <param name="parameters">The parameter types of the method. Leave empty if the method has no parameters.</param>
    /// <returns>The <see cref="MethodInfo"/> object if a method with the specified signature is found; otherwise, <c>null</c>.</returns>
    public static IEnumerable<MethodInfo> GetManyMethodInfo(this Type type, string name, Type returnType, params Type[]? parameters)
    {
        foreach (var methodInfo in type.GetMethods())
        {
            if (methodInfo.Name != name)
            {
                continue;
            }
            if (!TypeComparer.GenericCompare(returnType, methodInfo.ReturnType))
            {
                continue;
            }

            var methodParameters = methodInfo.GetParameters();

            if (parameters?.Length != methodParameters.Length)
            {
                continue;
            }

            bool continueParent = false;

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var methodParameter = methodParameters[i].ParameterType;

                if (methodParameter.GenericTypeArguments.Length != parameter.GenericTypeArguments.Length)
                {
                    continueParent = true;
                    break;
                }
                if (!TypeComparer.GenericCompare(parameter, methodParameter))
                {
                    continueParent = true;
                    break; ;
                }
            }

            if (continueParent)
            {
                continue;
            }

            yield return methodInfo;
        }
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
    public static MethodInfo? GetMethodInfo(this Type self, string name, Type[] parameters, Type outputType)
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

    /// <summary>
    /// Attempts to find a method in the provided type with a given signature.
    /// </summary>
    /// <param name="type">The type in which to search for the method.</param>
    /// <param name="name">The name of the method to search for.</param>
    /// <param name="returnType">The return type of the method. Use <c>null</c> for methods that return <c>void</c>.</param>
    /// <param name="parameters">The parameter types of the method. Leave empty if the method has no parameters.</param>
    /// <returns>The <see cref="MethodInfo"/> object if a method with the specified signature is found; otherwise, <c>null</c>.</returns>
    public static MethodInfo? GetMethodInfo(this Type type, string name, Type returnType, params Type[]? parameters)
    {
        return GetManyMethodInfo(type, name, returnType, parameters).FirstOrDefault();
    }
}
