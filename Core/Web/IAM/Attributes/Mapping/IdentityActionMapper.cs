using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Attributes;

/// <summary>
/// Provides utility methods to map and validate action methods with the <see cref="IdentityActionAttribute"/>.
/// </summary>
public class IdentityActionMapper
{
    /// <summary>
    /// Retrieves action methods annotated with <see cref="IdentityActionAttribute"/> and validates their configuration.
    /// </summary>
    /// <param name="settings">Optional settings to customize the behavior of the mapper.</param>
    /// <returns>An enumerable of <see cref="IdentityActionAttribute"/> associated with action methods.</returns>
    public static IEnumerable<IdentityActionAttribute> GetRouteActionAttributes(Settings? settings = null)
    {
        settings ??= new Settings();

        foreach (var methodInfo in GetRoutesMethodInfo())
        {
            var controllerType = methodInfo.ReflectedType ?? methodInfo.DeclaringType;
            var controllerName = controllerType?.FullName;

            var identityAttributes = methodInfo
                .GetCustomAttributes()
                .Where(x => x is IdentityActionAttribute)
                .ToArray();

            var anonymousAttributes = methodInfo
                .GetCustomAttributes()
                .Where(x => x is AnonymousActionAttribute)
                .ToArray();

            var nonActionAttributes = methodInfo
                .GetCustomAttributes()
                .Where(methodInfo => methodInfo is NonActionAttribute)
                .ToArray();

            if (nonActionAttributes.IsNotEmpty() && !settings.IgnoreNonActionAttribute)
            {
                continue;
            }

            if (anonymousAttributes.IsNotEmpty() && identityAttributes.IsNotEmpty())
            {
                throw new InvalidOperationException($"The action method \"{controllerName}.{methodInfo.Name}\" has both [IdentityAction] and [AnonymousAction] attributes applied. Due to security constraints, these attributes are mutually exclusive. Please review and correct this configuration.");
            }

            if (anonymousAttributes.IsNotEmpty() && !settings.IgnoreAnonymousAttribute)
            {
                continue;
            }

            if (identityAttributes.IsEmpty())
            {
                if (!settings.AllowActionessRoute)
                {
                    throw new Exception($"The action method \"{controllerName}.{methodInfo.Name}\" is missing the [IdentityAction] attribute. Please annotate it using: [IdentityAction(\"domain:resource:action\")] for proper access control.");
                }

                continue;
            }

            if (identityAttributes.Length > 1)
            {
                throw new Exception($"The action method \"{controllerName}.{methodInfo.Name}\" has been annotated with multiple [IdentityAction] attributes. An action method should have only one [IdentityAction] attribute. Please review and correct this configuration.");
            }

            yield return identityAttributes.First()
                .TypeCast<IdentityActionAttribute>();
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

    /// <summary>
    /// Represents configuration settings for the <see cref="IdentityActionMapper"/>.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets or sets a value indicating whether action methods without <see cref="IdentityActionAttribute"/>, or <see cref="AnonymousActionAttribute"/> are allowed.
        /// </summary>
        public bool AllowActionessRoute { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to ignore all <see cref="AnonymousActionAttribute"/> attributes.
        /// </summary>
        public bool IgnoreAnonymousAttribute { get; set; } = false;

        public bool IgnoreNonActionAttribute { get; set;} = false;
    }
}