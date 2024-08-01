namespace ModularSystem.Web.AccessManagement.Attributes;

/// <summary>
/// Serves as a base for attributes related to access management within an ASP.NET Core application. <br/>
/// This abstract class is the foundation for attributes that control access to controllers and action methods <br/>
/// based on authorization policies or exclusion from such policies.
/// </summary>
public abstract class AccessManagementAttribute : Attribute
{
    public abstract IAccessPolicy GetAccessPolicy();
}

[AttributeUsage(AttributeTargets.Class)]
public class AuthorizeControllerAttribute : AccessManagementAttribute
{
    public string Permission { get; }

    public AuthorizeControllerAttribute(string permission)
    {
        Permission = permission;
    }

    public override IAccessPolicy GetAccessPolicy()
    {
        return new AccessPolicyBuilder()
            .AddRequiredPermission(new IdentityPermission(Permission))
            .Build();
    }
}

/// <summary>
/// Specifies authorization policies for a specific action method within a controller. <br/>
/// When applied, it requires that the caller meets the specified authorization criteria to execute the action.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeActionAttribute : AccessManagementAttribute
{
    public string Permission { get; }

    public AuthorizeActionAttribute(string permission)
    {
        Permission = permission;
    }

    public override IAccessPolicy GetAccessPolicy()
    {
        return new AccessPolicyBuilder()
            .AddRequiredPermission(new IdentityPermission(Permission))
            .Build();
    }
}

/// <summary>
/// Marks an action method as exempt from the application's general access control policies, allowing unrestricted access. <br/>
/// This attribute effectively nullifies the effect of <see cref="AuthorizeControllerAttribute"/> and <see cref="AuthorizeActionAttribute"/> on the marked action.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AnonymousActionAttribute : AccessManagementAttribute
{
    public override IAccessPolicy GetAccessPolicy()
    {
        return new AccessPolicy();
    }
}
