using ModularSystem.Core.AccessManagement;

namespace ModularSystem.Web.Attributes;

/// <summary>
/// Serves as a base for attributes related to access management within an ASP.NET Core application. <br/>
/// This abstract class is the foundation for attributes that control access to controllers and action methods <br/>
/// based on authorization policies or exclusion from such policies.
/// </summary>
public abstract class AccessManagementAttribute : Attribute
{
}

/// <summary>
/// Decorates a controller with authorization requirements, specifying the permissions necessary for accessing its actions.
/// </summary>
/// <remarks>
/// The <see cref="AuthorizeControllerAttribute"/> is used at the controller level to enforce a set of permissions across all action methods within the controller. 
/// This ensures that only users with the appropriate permissions can interact with the controller's endpoints. Permissions are defined as strings and can 
/// represent specific actions, resources, or roles within the system. When combined with a permissions prefix, this attribute allows for a structured and 
/// scalable approach to defining access control policies within an ASP.NET Core application.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class AuthorizeControllerAttribute : AccessManagementAttribute
{
    /// <summary>
    /// Optional prefix used to namespace permissions for concise attribute application.
    /// </summary>
    /// <example>
    /// For a controller managing user profiles in a "users" domain, a prefix of "users:profile:" could be used. Subsequent permissions 
    /// such as "read" or "update" can then be specified without repeating the common prefix, leading to cleaner and more maintainable code.
    /// </example>
    public string? PermissionsPrefix { get; }

    /// <summary>
    /// A list of permissions that are required to access any action within the controller.
    /// </summary>
    /// <example>
    /// In a controller managing financial records, permissions might include "financial:records:read" for accessing records 
    /// and "financial:records:write" for creating or modifying records. These permissions ensure that only appropriately authorized users 
    /// can perform sensitive financial operations.
    /// </example>
    public List<string> Permissions { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeControllerAttribute"/> with a specified set of permissions.
    /// </summary>
    /// <param name="prefix">An optional prefix to be prepended to each permission, allowing for namespace-like structuring of permissions.</param>
    /// <param name="permissions">The permissions required to access the controller's actions.</param>
    public AuthorizeControllerAttribute(string? prefix = null, params string[] permissions)
    {
        PermissionsPrefix = prefix;
        Permissions.AddRange(permissions);
    }
}

/// <summary>
/// Specifies authorization policies for a specific action method within a controller. <br/>
/// When applied, it requires that the caller meets the specified authorization criteria to execute the action.
/// </summary>
/// <remarks>
/// Use this attribute to override or supplement the permissions required by the <see cref="AuthorizeControllerAttribute"/> <br/>
/// at the controller level or to specify permissions for actions in controllers without a controller-level authorization attribute.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeActionAttribute : AccessManagementAttribute
{
    /// <summary>
    /// Gets the list of permissions required to execute the action method.
    /// </summary>
    public List<string> Permissions { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeActionAttribute"/> class with the specified permissions.
    /// </summary>
    /// <param name="permissions">A list of permissions required to execute the action method.</param>
    public AuthorizeActionAttribute(params string[] permissions)
    {
        Permissions.AddRange(permissions);
    }
}

/// <summary>
/// Marks an action method as exempt from the application's general access control policies, allowing unrestricted access. <br/>
/// This attribute effectively nullifies the effect of <see cref="AuthorizeControllerAttribute"/> and <see cref="AuthorizeActionAttribute"/> on the marked action.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AnonymousActionAttribute : AccessManagementAttribute
{
}
