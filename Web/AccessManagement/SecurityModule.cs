using ModularSystem.Core;
using ModularSystem.Core.Extensions;
using ModularSystem.Core.Patterns;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ModularSystem.Web.AccessManagement;

/// <summary>
/// Defines an interface for identity objects within the system, allowing for unique identification <br/>
/// and permission management. This is crucial for implementing secure access control mechanisms.
/// </summary>
public interface IIdentity
{
    /// <summary>
    /// Retrieves the collection of permissions associated with the identity. Permissions are used
    /// to determine what actions the identity can perform or what resources it can access.
    /// Example: A user identity may have permissions like "read:documents" or "write:documents",
    /// allowing for fine-grained access control.
    /// </summary>
    /// <returns>A collection of <see cref="IdentityPermission"/> objects.</returns>
    IEnumerable<IIdentityPermission> GetPermissions();
}

/// <summary>
/// Represents a permission within the access control system. Permissions are defined by a string
/// pattern that can include wildcards for flexible access control definitions.
/// </summary>
/// <remarks>
/// Permissions are structured hierarchically, separated by colons (:). Each segment of the permission
/// string represents a level in the hierarchy. Wildcards allow for broad matching at various levels:
/// <list type="bullet">
/// <item>
/// <description> <c>'*'</c> Matches any single segment at its level in the hierarchy, 
/// <br/>
/// allowing for any action or resource to be matched as long as no further hierarchy is specified.</description>
/// </item>
/// <item>
/// <description><c>'**'</c> Matches any number of segments from its position downwards, encompassing all
/// sub-hierarchies. 
/// <br/>
/// This recursive wildcard allows for a broad permission scope from a specific point
/// in the hierarchy.</description>
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Matches any action on "documents", but does not extend to deeper hierarchical levels.
/// var permission = new IdentityPermission("documents:*");
/// 
/// // Matches reading any document, including those within any sub-folders.
/// var deepReadPermission = new IdentityPermission("documents:**:read");
/// 
/// // Matches any action on "documents" and any sub-hierarchies.
/// var fullAccess = new IdentityPermission("documents:**");
/// 
/// // The permission "documents:*" would not match "documents:sensitive:read" because it specifies
/// // a deeper hierarchy level with "read". However, "documents:**" would match it, as the recursive
/// // wildcard includes all actions and sub-hierarchies from "documents" downwards.
/// </code>
/// </example>
public interface IIdentityPermission : IEquatable<IIdentityPermission>
{
    /// <summary>
    /// Retrieves the segments of the permission string, separated by colons. Each segment represents a hierarchical <br/>
    /// level of the permission structure. 
    /// <br/>
    /// Example: The permission string "documents:read" has two segments, "documents" and "read".
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetSegments();
}

/// <summary>
/// Represents a permission within the access control system. Permissions are defined by a string
/// </summary>
public class IdentityPermission : IIdentityPermission
{
    /// <summary>
    /// Represents a wildcard that matches any single level in the permission hierarchy.
    /// </summary>
    public const string Wildcard = "*";

    /// <summary>
    /// Represents a recursive wildcard that matches any number of levels in the permission hierarchy.
    /// </summary>
    public const string RecursiveWildcard = "**";

    /// <summary>
    /// Regular expression to validate segment patterns within permission strings. <br/>
    /// This ensures that permissions are well-formed according to the rules of the access control system.
    /// </summary>
    public static readonly Regex SegmentRegex = new Regex(@"^(\*\*|\*|[a-z]+([a-z0-9_-]*[a-z0-9]+)*)$", RegexOptions.Compiled);

    /// <summary>
    /// The segments of the permission string, separated by colons. Each segment represents a hierarchical <br/>
    /// level of the permission structure, allowing for complex and granular access control definitions.
    /// </summary>
    public List<string> Segments { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of IdentityPermission from a permission string, parsing it into hierarchical segments.
    /// Wildcards ('*') match any action or resource at their level, while recursive wildcards ('**') match all actions or
    /// resources under their level in the hierarchy.
    /// </summary>
    /// <remarks>
    /// For example, the permission string "documents:*" allows for any action on documents at the top level,
    /// but does not grant access to "documents:sensitive:read" because of the deeper "sensitive:read" hierarchy. <br/><br/>
    /// Conversely, "documents:**" grants access to any action under "documents", including any number of sub-levels,
    /// matching "documents:sensitive:read" or even deeper nested permissions.
    /// </remarks>
    /// <param name="permissionString">The permission string to parse into segments.</param>
    public IdentityPermission(string permissionString)
    {
        if (string.IsNullOrEmpty(permissionString))
        {
            throw new ArgumentException("Permission string must not be null or empty.", nameof(permissionString));
        }

        var split = permissionString.Split(":");

        foreach (var segment in split)
        {
            if (!SegmentRegex.IsMatch(segment))
            {
                var error = $"The segment '{segment}' in the permission string '{permissionString}' is not valid. Segments must start and end with a letter and can contain letters, digits, hyphens ('-'), and underscores ('_'), but underscores cannot be at the beginning or end.";

                throw new ArgumentException(error);
            }

            Segments.Add(segment);
        }
    }

    /*
     * Factory Methods
     */

    /// <summary>
    /// Creates a permission with a wildcard that matches any single level in the permission hierarchy. <br/>
    /// Example: A permission with "*" can match any single action or resource at a given level.
    /// </summary>
    public static IdentityPermission WildcardPermission()
    {
        return new IdentityPermission(Wildcard);
    }

    /// <summary>
    /// Creates a permission with a recursive wildcard that matches any number of levels in the permission hierarchy. <br/>
    /// Example: A permission with "**" can match all actions or resources under a given domain.
    /// </summary>
    public static IdentityPermission RecursiveWildcardPermission()
    {
        return new IdentityPermission(RecursiveWildcard);
    }

    /*
     * Interface Implementation
     */

    public IEnumerable<string> GetSegments()
    {
        return Segments;
    }

    public bool Equals(IIdentityPermission? other)
    {
        if (other is null)
        {
            return false;
        }

        return GetSegments().SequenceEqual(other.GetSegments());
    }

    /// <summary>
    /// Returns a string representation of the permission, with segments joined by colons.
    /// </summary>
    public override string ToString()
    {
        return string.Join(':', Segments);
    }

    /// <summary>
    /// Determines whether the specified <see cref="IdentityPermission"/> is equal to the current <see cref="IdentityPermission"/>. <br/>
    /// Equality is based on the sequence of segments.
    /// </summary>
    public bool Equals(IdentityPermission? other)
    {
        if (other is null)
        {
            return false;
        }

        return Segments.SequenceEqual(other.Segments);
    }

}

/// <summary>
/// Represents a policy that defines the required permissions for accessing a resource or performing an action. <br/>
/// Policies are used to enforce access control by specifying what permissions an identity must have.
/// </summary>
public interface IAccessPolicy
{
    bool Authorize(IIdentity identity);
}

/// <summary>
/// Represents a policy that defines the required permissions for accessing a resource or performing an action. <br/>
/// Policies are used to enforce access control by specifying what permissions an identity must have.
/// </summary>
public class AccessPolicy : IAccessPolicy
{
    /// <summary>
    /// The collection of permissions required by this policy. <br/>
    /// An identity must satisfy all required permissions to be authorized according to this policy. <br/>
    /// Example: An access policy may require both <c>`documents:read`</c> and <c>`documents:write`</c> permissions <br/>
    /// to grant full access to document resources.
    /// </summary>
    private IIdentityPermission[] RequiredPermissions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessPolicy"/> class with an optional collection
    /// of required permissions.
    /// </summary>
    /// <param name="requiredPermissions">The collection of permissions to initialize the policy with.</param>
    public AccessPolicy(IEnumerable<IIdentityPermission>? requiredPermissions)
    {
        RequiredPermissions = requiredPermissions?.ToArray() ?? Array.Empty<IIdentityPermission>();
    }

    /// <summary>
    /// Determines whether a given identity is authorized according to this policy. <br/>
    /// Authorization is granted if the identity has all required permissions defined by the policy. <br/>
    /// Example: A document editing feature may require an identity to have both "read:documents" 
    /// and "write:documents" permissions to be authorized.
    /// </summary>
    /// <param name="identity">The identity to authorize.</param>
    /// <returns><c>true</c> if the identity is authorized; otherwise, <c>false</c>.</returns>
    public virtual bool Authorize(IIdentity identity)
    {
        var identityPermissions = identity.GetPermissions();

        foreach (var requiredPermission in RequiredPermissions)
        {
            var satisfiedPermissions = identityPermissions
                .Where(identityPermission => EvaluatePermissions(requiredPermission, identityPermission));

            if (satisfiedPermissions.IsEmpty())
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Evaluates whether a given identity permission satisfies a required permission. <br/>
    /// This involves checking for exact matches or appropriate wildcard matches.
    /// </summary>
    /// <param name="requiredPermission">The required permission for access.</param>
    /// <param name="identityPermission">The permission held by the identity.</param>
    /// <returns><c>true</c> if the identity permission satisfies the required permission; otherwise, <c>false</c>.</returns>
    protected bool EvaluatePermissions(IIdentityPermission requiredPermission, IIdentityPermission identityPermission)
    {
        var requiredPermissionSegments = requiredPermission.GetSegments().ToArray();
        var identityPermissionSegments = identityPermission.GetSegments().ToArray();

        for (int i = 0; i < requiredPermissionSegments.Length; i++)
        {
            var requiredSegment = requiredPermissionSegments.ElementAt(i);
            var identitySegment = identityPermissionSegments.ElementAtOrDefault(i);

            if (identitySegment == null) return false;
            if (identitySegment == IdentityPermission.RecursiveWildcard) return true;
            if (identitySegment == IdentityPermission.Wildcard) continue;
            if (identitySegment != requiredSegment) return false;
        }
        return true;
    }
}

/// <summary>
/// Concrete implementation of IIdentity, encapsulating a unique identifier and associated permissions and roles. <br/>
/// This class serves as the foundation for identity objects within the system.
/// </summary>
/// <example>
/// Creating an identity for a user might involve specifying a unique username or ID, along with permissions like
/// ["documents:read", "documents:write"] and roles such as ["editor", "reviewer"].
/// </example>
public class Identity : IIdentity
{
    /// <summary>
    /// Collection of permissions associated with the identity. <br/>
    /// This property defines what actions the identity can perform or what resources it can access, <br/>
    /// forming the basis of the access control mechanism.
    /// </summary>
    public List<IIdentityPermission> Permissions { get; set; }

    /// <summary>
    /// Collection of role names associated with the identity. Roles are used to group <br/>
    /// permissions into meaningful sets that represent specific functions or capabilities <br/>
    /// within the system, simplifying permission management.
    /// </summary>
    public List<string> Roles { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Identity"/> class with specified
    /// unique identifier, permissions, and roles.
    /// </summary>
    /// <param name="permissions">The collection of permissions associated with the identity.</param>
    /// <param name="roles">The collection of role names associated with the identity.</param>
    public Identity(
        IEnumerable<IIdentityPermission> permissions,
        IEnumerable<string> roles)
    {
        Permissions = permissions.ToList();
        Roles = roles.ToList();
    }

    /// <summary>
    /// Retrieves the collection of permissions associated with this identity.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="IdentityPermission"/>.</returns>
    public IEnumerable<IIdentityPermission> GetPermissions() => Permissions;

}

/// <summary>
/// Represents a named role within the access control system. <br/>
/// A role groups multiple permissions together, allowing for easier management of access controls based on <br/>
/// common functions or responsibilities.
/// </summary>
public class IdentityRole
{
    /// <summary>
    /// The name of the role, used for identification and assignment to identities.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The list of permissions associated with this role. These permissions define <br/>
    /// the access rights granted to identities assigned this role.
    /// </summary>
    public List<IIdentityPermission> Permissions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityRole"/> class with a specified name 
    /// and an optional list of permissions.
    /// </summary>
    /// <param name="name">The name of the role.</param>
    /// <param name="permissions">An optional list of permissions to associate with this role.</param>
    [JsonConstructor]
    public IdentityRole(string name, IEnumerable<IIdentityPermission> permissions)
    {
        Name = name;
        Permissions = permissions?.ToList() ?? new();
    }

}

/// <summary>
/// Represents a specific action or operation within the system, with associated permissions
/// defining the access control requirements. This allows for detailed control over what
/// actions an identity can perform, based on its permissions.
/// </summary>
public class IdentityAction
{
    /// <summary>
    /// Gets or sets the name of the action.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the permissions required for executing this action on the specified resource.
    /// </summary>
    public IIdentityPermission[] RequiredPermissions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityAction"/> class based on an action string.
    /// </summary>
    /// <param name="name">The action string in the format "domain:resource:action".</param>
    /// <param name="requiredPermissions"></param>
    /// <remarks>
    /// The action string should adhere to the format "domain:resource:action". 
    /// It provides a structured way to quickly define actions and their scope.
    /// </remarks>
    [JsonConstructor]
    public IdentityAction(string name, IEnumerable<IIdentityPermission> requiredPermissions)
    {
        Name = name;
        RequiredPermissions = requiredPermissions.ToArray();
    }

    /// <summary>
    /// Returns a string that represents the current action.
    /// </summary>
    /// <returns>
    /// A string in the format "domain:resource:action".
    /// </returns>
    public override string ToString()
    {
        return $"{Name} - {RequiredPermissions.Length} required permissions.";
    }

    /// <summary>
    /// Retrieves the access policy associated with this action, detailing the permissions required to execute it.
    /// </summary>
    /// <returns>An <see cref="AccessPolicy"/> object that encapsulates the set of permissions required for this action.</returns>
    public virtual AccessPolicy GetAccessPolicy()
    {
        return new AccessPolicy(RequiredPermissions);
    }

}

public interface IIdentityProvider
{
    IOperationResult<IIdentity?> GetIdentity(HttpContent httpContent);
}

public interface IAccessPolicyProvider
{
    IAccessPolicy? GetAccessPolicy(HttpContent httpContent);
}

public class EmptyIdentity : IIdentity
{
    public IEnumerable<IIdentityPermission> GetPermissions()
    {
        return Array.Empty<IIdentityPermission>();
    }
}

public class EmptyAccessPolicy : IAccessPolicy
{
    public bool Authorize(IIdentity identity)
    {
        return true;
    }
}