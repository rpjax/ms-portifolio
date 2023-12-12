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
        return GetManyMethodInfo(type, name, returnType, parameters).FirstOrDefault();
    }

    /// <summary>
    /// Attempts to find a method in the provided type with a given signature.
    /// </summary>
    /// <param name="type">The type in which to search for the method.</param>
    /// <param name="name">The name of the method to search for.</param>
    /// <param name="returnType">The return type of the method. Use <c>null</c> for methods that return <c>void</c>.</param>
    /// <param name="parameters">The parameter types of the method. Leave empty if the method has no parameters.</param>
    /// <returns>The <see cref="MethodInfo"/> object if a method with the specified signature is found; otherwise, <c>null</c>.</returns>
    public static IEnumerable<MethodInfo> GetManyMethodInfo(this Type type, string name, Type? returnType, params Type[]? parameters)
    {
        var methods = type.GetMethods();

        foreach (var methodInfo in type.GetMethods())
        {
            if (methodInfo.Name != name)
            {
                continue;
            }
            if (returnType == null && methodInfo.ReturnType != typeof(void))
            {
                continue;
            }

            var methodParameters = methodInfo.GetParameters();

            if (parameters == null || parameters.Length != methodParameters.Length)
            {
                continue;
            }

            var parametersMatch = true;

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var methodParameter = methodParameters[i].ParameterType;

                if (parameter.GenericTypeArguments.IsNotEmpty() && methodParameter.IsGenericType)
                {
                    methodParameter = methodParameter
                        .GetGenericTypeDefinition()
                        .MakeGenericType(parameter.GenericTypeArguments);
                }
                if (methodParameter.IsGenericParameter)
                {
                    methodParameter = parameter;
                }

                if (parameter != methodParameter)
                {
                    parametersMatch = false;
                    break;
                }
            }

            if (!parametersMatch)
            {
                continue;
            }

            yield return methodInfo;
        }
    }
}
