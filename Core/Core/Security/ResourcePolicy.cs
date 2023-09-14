namespace ModularSystem.Core.Security;

/// <summary>
/// Represents a policy that determines whether a given identity has access to a resource.
/// </summary>
public interface IResourcePolicy
{
    /// <summary>
    /// Determines whether the provided identity is authorized to access the resource.
    /// </summary>
    /// <param name="identity">The identity to evaluate against the policy.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the identity is authorized.</returns>
    Task<bool> AuthorizeAsync(IIdentity identity);

    /// <summary>
    /// Attempts to authorize the provided identity to access the resource. If no identity is provided, the method should return false.
    /// </summary>
    /// <param name="identity">The identity to evaluate against the policy. It can be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the identity is authorized or not. If no identity is provided, it should return false.</returns>
    Task<bool> TryAuthorizeAsync(IIdentity? identity);
}

/// <summary>
/// Represents a resource policy that grants unrestricted access to any identity. 
/// It effectively acts as a bypass for resource authorization checks.
/// </summary>
public class EmptyResourcePolicy : IResourcePolicy
{
    /// <summary>
    /// Asynchronously authorizes the provided identity to access a resource, 
    /// always returning true regardless of the identity's permissions.
    /// </summary>
    /// <param name="identity">The identity to evaluate against the policy.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result always contains a value of true, 
    /// indicating that the identity is authorized.
    /// </returns>
    public Task<bool> AuthorizeAsync(IIdentity identity)
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// Asynchronously attempts to authorize the provided identity to access a resource,
    /// always returning true regardless of the identity's presence or permissions.
    /// </summary>
    /// <param name="identity">The identity to evaluate against the policy, which can be null.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result always contains a value of true, 
    /// indicating that the identity is authorized or no authorization check is required.
    /// </returns>
    public Task<bool> TryAuthorizeAsync(IIdentity? identity)
    {
        return Task.FromResult(true);
    }
}

/// <summary>
/// Represents a policy that evaluates whether a given identity has the necessary permissions to access a resource.
/// </summary>
public class ResourcePolicy : IResourcePolicy
{
    protected static readonly IdentityPermission SystemPermission = DefinedPermissions.CreateSystemPermission();

    /// <summary>
    /// Gets or sets a value indicating whether system-level permissions should bypass the authorization checks.
    /// </summary>
    public bool UseSystemBypass { get; set; } = true;

    /// <summary>
    /// List of permissions required for the resource access.
    /// </summary>
    protected List<IdentityPermission> RequiredPermissions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourcePolicy"/> class with an optional set of required permissions.
    /// </summary>
    /// <param name="permissions">The permissions required to access the resource. If not provided, no permissions are set as default.</param>
    public ResourcePolicy(IEnumerable<IdentityPermission>? permissions = null)
    {
        RequiredPermissions = permissions?.ToList() ?? new();
    }

    /// <summary>
    /// Sets a single permission as required for the resource.
    /// </summary>
    /// <param name="permission">The required permission.</param>
    /// <returns>Returns the current instance, allowing for method chaining.</returns>
    public ResourcePolicy SetRequiredPermission(IdentityPermission permission)
    {
        RequiredPermissions.Add(permission);
        return this;
    }

    /// <summary>
    /// Sets a collection of permissions as required for the resource.
    /// </summary>
    /// <param name="permissions">The collection of required permissions.</param>
    /// <returns>Returns the current instance, allowing for method chaining.</returns>
    public ResourcePolicy SetRequiredPermissions(IEnumerable<IdentityPermission> permissions)
    {
        RequiredPermissions.AddRange(permissions);
        return this;
    }

    /// <summary>
    /// Evaluates if the provided identity is authorized to access the resource.
    /// </summary>
    /// <param name="identity">The identity to evaluate against the policy.</param>
    /// <returns>True if the identity is authorized; otherwise, false.</returns>
    public virtual bool Authorize(IIdentity identity)
    {
        foreach (var permission in RequiredPermissions)
        {
            if (!FindPermission(identity, permission))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Attempts to authorize the provided identity to access the resource. 
    /// Handles potential absence of identity and empty permission requirements.
    /// </summary>
    /// <param name="identity">The identity to evaluate against the policy, which can be null.</param>
    /// <returns>True if the identity is authorized or no permissions are required; otherwise, false.</returns>
    public virtual bool TryAuthorize(IIdentity? identity)
    {
        if (RequiredPermissions.IsEmpty())
        {
            return true;
        }

        if (identity == null)
        {
            return false;
        }

        return Authorize(identity);
    }

    /// <summary>
    /// Asynchronously determines if the provided identity is authorized to access the resource.
    /// </summary>
    /// <param name="identity">The identity to evaluate against the policy.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the identity is authorized.</returns>
    public virtual Task<bool> AuthorizeAsync(IIdentity identity)
    {
        return Task.FromResult(Authorize(identity));
    }

    /// <summary>
    /// Asynchronously attempts to authorize the provided identity to access the resource.
    /// </summary>
    /// <param name="identity">The identity to evaluate against the policy, which can be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the identity is authorized or not.</returns>
    public virtual Task<bool> TryAuthorizeAsync(IIdentity? identity)
    {
        return Task.FromResult(TryAuthorize(identity));
    }

    /// <summary>
    /// Determines if the provided identity has a specific permission.
    /// </summary>
    /// <param name="identity">The identity to check.</param>
    /// <param name="requriedPermission">The permission to find.</param>
    /// <returns>True if the identity has the required permission; otherwise, false.</returns>
    protected virtual bool FindPermission(IIdentity identity, IdentityPermission requriedPermission)
    {
        foreach (var identityPermission in identity.GetPermissions())
        {
            if (UseSystemBypass && identityPermission.Equals(SystemPermission))
            {
                return true;
            }

            var serviceMatch =
            identityPermission.Domain == IdentityPermission.Wildcard ||
            identityPermission.Domain == requriedPermission.Domain;

            var resourceMatch =
                identityPermission.Resource == IdentityPermission.Wildcard ||
                identityPermission.Resource == requriedPermission.Resource;

            var actionMatch =
                identityPermission.Name == IdentityPermission.Wildcard ||
                identityPermission.Name == requriedPermission.Name;

            var identityHasPermission = serviceMatch && resourceMatch && actionMatch;

            if (identityHasPermission)
            {
                return true;
            }
        }

        return false;
    }
}