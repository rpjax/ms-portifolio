using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using ModularSystem.Core;
using ModularSystem.Core.AccessManagement;
using System.Reflection;

namespace ModularSystem.Web.Attributes;

/// <summary>
/// Scans MVC controllers and action methods in the application to map each action to its route and associated permissions, <br/>
/// ensuring that access control policies are enforced according to the specified attributes.
/// </summary>
public class IdentityActionsMapper
{
    /// <summary>
    /// Scans all MVC controllers and action methods to construct a collection of IdentityAction objects, <br/>
    /// each representing a route and the permissions required to access it.
    /// </summary>
    /// <param name="settings">Optional settings to customize the mapper's behavior. Allows excluding routes without explicit access management attributes.</param>
    /// <returns>A collection of IdentityAction objects, each corresponding to a controller action method.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a method is improperly configured with both [AuthorizeAction] and [AnonymousAction] attributes, or if a route cannot be determined.</exception>
    public static IEnumerable<IdentityAction> GetIdentityActions(Settings? settings = null)
    {
        settings ??= new Settings();

        foreach (var methodInfo in GetRoutesMethodInfo())
        {
            var controllerType = methodInfo.ReflectedType ?? methodInfo.DeclaringType;
            var controllerName = controllerType?.FullName;
            var route = GetRoute(methodInfo);

            if (controllerType == null)
            {
                throw new InvalidOperationException();
            }

            if (route == null)
            {
                throw new InvalidOperationException($"Unable to determine route for action method \"{controllerName}.{methodInfo.Name}\".");
            }

            var controllerAuthorizeAttribute = controllerType.GetCustomAttribute<AuthorizeControllerAttribute>();
            var authorizeAttribute = methodInfo.GetCustomAttribute<AuthorizeActionAttribute>();
            var anonymousAttribute = methodInfo.GetCustomAttribute<AnonymousActionAttribute>();

            if (anonymousAttribute is not null && authorizeAttribute is not null)
            {
                throw new InvalidOperationException($"The action method \"{controllerName}.{methodInfo.Name}\" has both [AuthorizeAction] and [AnonymousAction] attributes applied. Due to security constraints, these attributes are mutually exclusive. Please review and correct this configuration.");
            }

            if (anonymousAttribute is not null)
            {
                continue;
            }

            if (authorizeAttribute is null)
            {
                if (!settings.AllowAttributelessRoute)
                {
                    throw new Exception($"The action method \"{controllerName}.{methodInfo.Name}\" is missing the access management attribute. Please annotate it using: [AuthorizeAction], or [AnonymousAction], for proper access control.");
                }

                continue;
            }

            var prefix = controllerAuthorizeAttribute?.PermissionsPrefix;
            var permissionStrings = authorizeAttribute.Permissions;

            if(!string.IsNullOrEmpty(prefix))
            {
                for (int i = 0; i < permissionStrings.Count; i++)
                {
                    permissionStrings[i] = $"{prefix}{permissionStrings[i]}";
                }
            }

            if(controllerAuthorizeAttribute is not null)
            {
                permissionStrings.AddRange(controllerAuthorizeAttribute.Permissions);
            }

            var permissions = permissionStrings
                .Transform(x => new IdentityPermission(x));

            yield return new IdentityAction(route, permissions);
        }
    }


    ///// <summary>
    ///// Retrieves a collection of routes for action methods that comply with the specified settings.
    ///// </summary>
    ///// <param name="settings">Optional. Settings to apply during the route retrieval process. Allows for customization of behavior, such as handling routes without access management attributes.</param>
    ///// <returns>An enumerable collection of strings, each representing a unique route associated with an action method.</returns>
    ///// <exception cref="InvalidOperationException">Thrown if an action method is improperly configured with both [AuthorizeAction] and [AnonymousAction] attributes, or if an action method requires an access management attribute but none is provided.</exception>
    //public static IEnumerable<string> GetActionRoutes(Settings? settings = null)
    //{
    //    settings ??= new Settings();

    //    foreach (var methodInfo in GetRoutesMethodInfo())
    //    {
    //        var controllerType = methodInfo.ReflectedType ?? methodInfo.DeclaringType;
    //        var controllerName = controllerType?.FullName;
    //        var route = GetRoute(methodInfo);

    //        if (route == null)
    //        {
    //            throw new InvalidOperationException($"Unable to determine route for action method \"{controllerName}.{methodInfo.Name}\".");
    //        }

    //        var authorizeAttributes = methodInfo
    //            .GetCustomAttributes()
    //            .Where(x => x is AccessManagementAttribute)
    //            .ToArray();

    //        var anonymousAttributes = methodInfo
    //            .GetCustomAttributes()
    //            .Where(x => x is AnonymousActionAttribute)
    //            .ToArray();

    //        if (anonymousAttributes.Any() && authorizeAttributes.Any())
    //        {
    //            throw new InvalidOperationException($"The action method \"{controllerName}.{methodInfo.Name}\" has both [AuthorizeAction] and [AnonymousAction] attributes applied. Due to security constraints, these attributes are mutually exclusive. Please review and correct this configuration.");
    //        }

    //        if (anonymousAttributes.Any())
    //        {
    //            continue;
    //        }

    //        if (!authorizeAttributes.Any())
    //        {
    //            if (!settings.AllowAttributelessRoute)
    //            {
    //                throw new Exception($"The action method \"{controllerName}.{methodInfo.Name}\" is missing the access management attribute. Please annotate it using: [AuthorizeAction], or [AnonymousAction], for proper access control.");
    //            }

    //            continue;
    //        }

    //        yield return route;
    //    }
    //}

    /// <summary>
    /// Identifies controller types within the current application domain that derive from ControllerBase and are not abstract.
    /// </summary>
    /// <returns>An enumerable collection of controller types.</returns>
    private static IEnumerable<Type> GetMvcControllers()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(ControllerBase)) && !type.IsAbstract);
    }

    /// <summary>
    /// Retrieves method info objects for all action methods within MVC controllers that have route information.
    /// </summary>
    /// <returns>An enumerable collection of MethodInfo objects, each representing an action method.</returns>
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
    /// Determines whether a given method is associated with an HTTP route.
    /// </summary>
    /// <param name="methodInfo">The MethodInfo object to evaluate.</param>
    /// <returns>true if the method has an HTTP route; otherwise, false.</returns>
    private static bool IsHttpRouteMethod(MethodInfo methodInfo)
    {
        return methodInfo.GetCustomAttributes().Any(att => att is HttpMethodAttribute);
    }

    /// <summary>
    /// Constructs the route for a given action method by combining controller and method-specific route templates.
    /// </summary>
    /// <param name="method">The MethodInfo object representing the action method.</param>
    /// <returns>A string representing the constructed route or null if no route can be determined.</returns>
    private static string? GetRoute(MethodInfo method)
    {
        var route = null as string;
        var controllerRouteAttribute = method.DeclaringType?.GetCustomAttribute<RouteAttribute>();

        if (controllerRouteAttribute != null)
        {
            route = controllerRouteAttribute.Template;
        }

        var methodRouteAttribute = method.GetCustomAttribute<RouteAttribute>();

        if (methodRouteAttribute != null)
        {
            route = string.IsNullOrEmpty(route)
                ? methodRouteAttribute.Template
                : $"{route.TrimEnd('/')}/{methodRouteAttribute.Template.TrimStart('/')}";
        }
        else
        {
            var httpMethodAttribute = method.GetCustomAttributes()
                .FirstOrDefault(attr => attr is HttpMethodAttribute) as HttpMethodAttribute;

            if (httpMethodAttribute != null && !string.IsNullOrEmpty(httpMethodAttribute.Template))
            {
                route = string.IsNullOrEmpty(route)
                    ? httpMethodAttribute.Template
                    : $"{route.TrimEnd('/')}/{httpMethodAttribute.Template.TrimStart('/')}";
            }
        }

        return route;
    }

    /// <summary>
    /// Provides settings for controlling the behavior of the IdentityActionsMapper.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Determines whether routes without explicit access management attributes are allowed.
        /// </summary>
        public bool AllowAttributelessRoute { get; set; } = false;
    }
}
