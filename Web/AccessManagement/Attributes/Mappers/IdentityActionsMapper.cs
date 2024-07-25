using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using ModularSystem.Core;
using ModularSystem.Core.AccessManagement;
using System.Reflection;

namespace ModularSystem.Web.Attributes;

/// <summary>
/// Represents an action within an ASP.NET Core MVC application with associated access control policies.
/// </summary>
/// <remarks>
/// This class encapsulates the routing information, the HTTP method, and the required permissions
/// for accessing a specific controller action. It is primarily used by the IdentityActionsMapper
/// to construct a comprehensive map of all controller actions within the application alongside
/// their access control requirements. This map is then utilized to enforce access control policies
/// according to the specified attributes on the controller actions.
/// </remarks>
public class HttpIdentityAction
{
    /// <summary>
    /// Gets the route template associated with the controller action.
    /// </summary>
    public string Route { get; }

    /// <summary>
    /// Gets the HTTP method (GET, POST, PUT, etc.) associated with the controller action.
    /// </summary>
    public HttpMethod Method { get; }

    /// <summary>
    /// Gets the collection of IdentityPermission objects required for accessing the controller action.
    /// </summary>
    public IdentityPermission[] RequiredPermissions { get; }

    /// <summary>
    /// Initializes a new instance of the HttpIdentityAction class with specified route, HTTP method, and required permissions.
    /// </summary>
    /// <param name="route">The route template associated with the controller action.</param>
    /// <param name="method">The HTTP method associated with the controller action.</param>
    /// <param name="requiredPermissions">An array of IdentityPermission objects required for accessing the controller action.</param>
    public HttpIdentityAction(string route, HttpMethod method, IdentityPermission[] requiredPermissions)
    {
        Route = route;
        Method = method;
        RequiredPermissions = requiredPermissions;
    }

    /// <summary>
    /// Constructs an AccessPolicy object based on the required permissions for the controller action.
    /// </summary>
    /// <returns>An AccessPolicy object representing the access control requirements for the controller action.</returns>
    public AccessPolicy GetAccessPolicy()
    {
        return new AccessPolicy(RequiredPermissions);
    }

    /// <summary>
    /// Returns a string that represents the current HttpIdentityAction object.
    /// </summary>
    /// <returns>A string that contains the HTTP method and route template of the controller action.</returns>
    public override string ToString()
    {
        return $"{Method} {Route}";
    }
}

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
    public static IEnumerable<HttpIdentityAction> GetHttpIdentityActions(Settings? settings = null)
    {
        settings ??= new Settings();

        foreach (var methodInfo in GetAllHttpActionMethodInfos())
        {
            var controllerType = (methodInfo.ReflectedType ?? methodInfo.DeclaringType)!;
            var controllerName = controllerType.FullName ?? controllerType.Name;
            var route = GetRoute(methodInfo);
            var method = GetHttpMethod(methodInfo);

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
                    throw new ErrorException($"The action method \"{controllerName}.{methodInfo.Name}\" is missing the access management attribute. Please annotate it using: [AuthorizeAction], or [AnonymousAction], for proper access control.");
                }

                continue;
            }

            var prefix = controllerAuthorizeAttribute?.PermissionsPrefix;
            var permissionStrings = authorizeAttribute.Permissions;

            if (!string.IsNullOrEmpty(prefix))
            {
                for (int i = 0; i < permissionStrings.Count; i++)
                {
                    permissionStrings[i] = $"{prefix}{permissionStrings[i]}";
                }
            }

            if (controllerAuthorizeAttribute is not null)
            {
                permissionStrings.AddRange(controllerAuthorizeAttribute.Permissions);
            }

            var permissions = permissionStrings
                .Transform(x => new IdentityPermission(x))
                .ToArray();

            if (permissions.IsEmpty())
            {
                throw new ErrorException($"The action method \"{controllerName}.{methodInfo.Name}\" is configured with access control attributes but did not resolve to any permissions. This effectively leaves the resource unprotected. To explicitly mark this resource as publicly accessible without permissions, please annotate it with the [AnonymousAction] attribute. Ensure the access management attributes ([AuthorizeAction], etc.) are correctly configured to generate the intended permissions.");
            }

            yield return new HttpIdentityAction(route, method, permissions);
        }
    }

    private static IEnumerable<Type> GetAllMvcControllerTypes()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(ControllerBase)) && !type.IsAbstract);
    }

    private static IEnumerable<MethodInfo> GetAllHttpActionMethodInfos()
    {
        foreach (var type in GetAllMvcControllerTypes())
        {
            var methods = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(methodInfo => IsHttpRouteMethod(methodInfo));

            foreach (var method in methods)
            {
                yield return method;
            }
        }
    }

    private static bool IsHttpRouteMethod(MethodInfo methodInfo)
    {
        return methodInfo.GetCustomAttributes().Any(att => att is HttpMethodAttribute);
    }

    private static string GetRoute(MethodInfo methodInfo)
    {
        // Obtem a rota base do controlador
        var controllerRoute = methodInfo.DeclaringType?.GetCustomAttribute<RouteAttribute>()?.Template ?? string.Empty;

        // Inicializa a rota do método com um valor padrão vazio
        var methodRoute = string.Empty;

        // Verifica e obtém a rota definida no método, considerando tanto RouteAttribute quanto os atributos específicos do método HTTP
        var routeAttribute = methodInfo.GetCustomAttribute<RouteAttribute>();
        if (routeAttribute != null)
        {
            methodRoute = routeAttribute.Template;
        }
        else
        {
            // Procura por atributos específicos do método HTTP que possam conter a definição de rota
            var httpMethodAttribute = methodInfo.GetCustomAttributes<HttpMethodAttribute>().FirstOrDefault(attr => !string.IsNullOrWhiteSpace(attr.Template));
            if (httpMethodAttribute != null)
            {
                methodRoute = httpMethodAttribute.Template;
            }
        }

        // Concatena a rota do controlador com a rota do método, se necessário
        var fullRoute = controllerRoute;
        if (!string.IsNullOrEmpty(methodRoute))
        {
            // Garante que tanto a rota do controlador quanto a rota do método estejam corretamente formatadas antes de concatená-las
            fullRoute = $"{controllerRoute.TrimEnd('/')}/{methodRoute.TrimStart('/')}";
        }

        if (string.IsNullOrEmpty(fullRoute))
        {
            throw new InvalidOperationException($"Unable to determine route for action method \"{methodInfo.DeclaringType?.FullName}.{methodInfo.Name}\".");
        }

        if(!fullRoute.StartsWith("/"))
        {
            fullRoute = $"/{fullRoute}";
        }

        return fullRoute;
    }

    private static HttpMethod GetHttpMethod(MethodInfo method)
    {
        var httpMethodAttribute = method.GetCustomAttributes<HttpMethodAttribute>(true).FirstOrDefault();

        if (httpMethodAttribute == null)
        {
            throw new InvalidOperationException($"HTTP method attribute not found for action method \"{method.DeclaringType?.FullName}.{method.Name}\".");
        }

        if (httpMethodAttribute is HttpDeleteAttribute)
        {
            return HttpMethod.Delete;
        }
        if (httpMethodAttribute is HttpGetAttribute)
        {
            return HttpMethod.Get;
        }
        if (httpMethodAttribute is HttpHeadAttribute)
        {
            return HttpMethod.Head;
        }
        if (httpMethodAttribute is HttpOptionsAttribute)
        {
            return HttpMethod.Options;
        }
        if (httpMethodAttribute is HttpPatchAttribute)
        {
            return HttpMethod.Patch;
        }
        if (httpMethodAttribute is HttpPostAttribute)
        {
            return HttpMethod.Post;
        }
        if (httpMethodAttribute is HttpPutAttribute)
        {
            return HttpMethod.Put;
        }

        throw new InvalidOperationException($"Unsupported HTTP method attribute for action method \"{method.DeclaringType?.FullName}.{method.Name}\".");
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
