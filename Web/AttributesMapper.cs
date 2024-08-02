using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Aidan.Core;
using System.Reflection;

namespace Aidan.Web;

public static class AttributesMapper
{
    public static IEnumerable<T> GetAttributesFromMvcActions<T>()
    {
        foreach (var methodInfo in GetRoutesMethodInfo())
        {
            var declaringType = methodInfo.DeclaringType;
            var controllerName = declaringType?.Name;

            var attributes = methodInfo.GetCustomAttributes()
            .Where(x => x is T)
            .ToArray();

            if (attributes.IsEmpty())
            {
                continue;
            }

            yield return attributes.First()
                .TypeCast<T>();
        }
    }

    /// <summary>
    /// Retrieves all MVC controller types from the current application domain.
    /// </summary>
    /// <returns>An enumerable of MVC controller types.</returns>
    private static IEnumerable<Type> GetMvcControllers()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(ControllerBase)) && !type.IsAbstract);
    }

    /// <summary>
    /// Retrieves action methods from MVC controllers that are potential HTTP route endpoints.
    /// </summary>
    /// <returns>An enumerable of action method info.</returns>
    private static IEnumerable<MethodInfo> GetRoutesMethodInfo()
    {
        foreach (var type in GetMvcControllers())
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(methodInfo => IsHttpRouteMethod(methodInfo));

            foreach (var method in methods)
            {
                yield return method;
            }
        }
    }

    /// <summary>
    /// Determines if the given method is an HTTP route method based on its attributes.
    /// </summary>
    /// <param name="methodInfo">The method to check.</param>
    /// <returns>True if the method is an HTTP route method; otherwise, false.</returns>
    private static bool IsHttpRouteMethod(MethodInfo methodInfo)
    {
        return methodInfo.GetCustomAttributes().Any(att => att is HttpMethodAttribute);
    }
}