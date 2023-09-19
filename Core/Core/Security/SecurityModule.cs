using System.Text.Json.Serialization;

namespace ModularSystem.Core.Security;

/// <summary>
/// Represents a user or system identity, providing an abstraction for accessing identity-related details.
/// </summary>
public interface IIdentity
{
    /// <summary>
    /// Gets or sets a unique identifier for the identity. This identifier is used to uniquely <br/>
    /// distinguish different identities within the system.
    /// </summary>
    string UniqueIdentifier { get; set; }

    /// <summary>
    /// Retrieves the set of permissions associated with this identity. Permissions define <br/>
    /// what actions or resources the identity has access to.
    /// </summary>
    /// <returns>A collection of <see cref="IdentityPermission"/> indicating the granted permissions.</returns>
    IEnumerable<IdentityPermission> GetPermissions();
}

/// <summary>
/// Defines permissions for a named resource within a named service.
/// </summary>
public class IdentityPermission : IEquatable<IdentityPermission>
{
    /// <summary>
    /// Represents a wildcard value, used to signify a broad or universal permission.
    /// </summary>
    public const string Wildcard = "*";

    /// <summary>
    /// Represents the domain or service to which this permission applies.
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// Represents the specific resource within the domain or service to which this permission applies.
    /// </summary>
    public string Resource { get; set; }

    /// <summary>
    /// The name of the permission.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// List of flags associated with the permission. Flags allow for extended behavior beyond the default permission set.
    /// </summary>
    public List<Flag> Flags { get; set; }

    /// <summary>
    /// Initializes an empty instance of <see cref="IdentityPermission"/> with all values set to <see cref="string.Empty"/>.
    /// </summary>
    [JsonConstructor]
    public IdentityPermission()
    {
        Domain = string.Empty;
        Resource = string.Empty;
        Name = string.Empty;
        Flags = new();
    }

    /// <summary>
    /// Constructs an IdentityPermission with specified domain, resource, and name.
    /// Optionally, a list of flags can be provided.
    /// </summary>
    public IdentityPermission(string domain, string resource, string name, IEnumerable<Flag>? flags = null)
    {
        Domain = domain;
        Resource = resource;
        Name = name;
        Flags = flags?.ToList() ?? new();
    }

    /// <summary>
    /// Constructs an IdentityPermission based on a permission string in the format "domain:resource:name".
    /// Optionally, a list of flags can be provided.
    /// </summary>
    public IdentityPermission(string permission, IEnumerable<Flag>? flags = null)
    {
        var split = permission.Split(':');

        if (split.Length != 3)
        {
            throw new ArgumentException(nameof(permission));
        }

        Domain = split[0];
        Resource = split[1];
        Name = split[2];
        Flags = flags?.ToList() ?? new();
    }

    /// <summary>
    /// Creates a wildcard permission that signifies a broad or universal permission.
    /// </summary>
    public static IdentityPermission WildcardPermission(string? permission = null, List<Flag>? flags = null)
    {
        return new IdentityPermission(Wildcard, Wildcard, permission ?? Wildcard, flags);
    }

    /// <summary>
    /// Adds a flag to this permission.
    /// </summary>
    /// <param name="key">The flag name.</param>
    /// <param name="value">The flag value.</param>
    /// <returns>Returns the IdentityPermission instance with the added flag.</returns>
    public IdentityPermission AddFlag(string key, string value)
    {
        Flags.Add(new Flag(key, value));
        return this;
    }

    /// <summary>
    /// Returns a string representation of this permission in the format "domain:resource:name".
    /// </summary>
    public override string ToString()
    {
        return $"{Domain}:{Resource}:{Name}";
    }

    /// <summary>
    /// Checks for equality with another IdentityPermission based on domain, resource, and name.
    /// </summary>
    public bool Equals(IdentityPermission? other)
    {
        return
            Domain == other?.Domain &&
            Resource == other?.Resource &&
            Name == other?.Name;
    }

    /// <summary>
    /// Represents a flag associated with an IdentityPermission. It provides a way to extend the default behavior of a permission.
    /// </summary>
    public class Flag
    {
        /// <summary>
        /// The name or key of the flag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value associated with the flag.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Constructs a flag with a specified name and value.
        /// </summary>
        [JsonConstructor]
        public Flag(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}

/// <summary>
/// Represents the default implementation of <see cref="IIdentity"/>.
/// </summary>
/// <remarks>
/// This implementation provides methods to associate permissions and roles with the identity.
/// </remarks>
public class Identity : IIdentity
{
    /// <summary>
    /// Gets or sets the unique identifier for this identity.
    /// </summary>
    public string UniqueIdentifier { get; set; }

    /// <summary>
    /// Gets or sets the list of permissions associated with this identity.
    /// </summary>
    public List<IdentityPermission> Permissions { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of role names associated with this identity.
    /// </summary>
    /// <remarks>
    /// This list contains the names of the roles that the identity belongs to. It helps in quickly determining <br/>
    /// the role membership of the identity without the need to evaluate permissions. 
    /// </remarks>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Initializes an empty instance of <see cref="Identity"/>.
    /// </summary>
    [JsonConstructor]
    public Identity()
    {
        UniqueIdentifier = string.Empty;
        Permissions = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Identity"/> class using a unique identifier.
    /// </summary>
    /// <param name="uniqueIdentifier">The unique identifier for the identity.</param>
    public Identity(string uniqueIdentifier)
    {
        UniqueIdentifier = uniqueIdentifier;
    }

    /// <summary>
    /// Returns the list of permissions associated with this identity.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="IdentityPermission"/>.</returns>
    public IEnumerable<IdentityPermission> GetPermissions()
    {
        return Permissions;
    }

    /// <summary>
    /// Associates a single permission with the identity, if it's not already present.
    /// </summary>
    /// <param name="permission">The permission to associate with the identity.</param>
    /// <returns>The current <see cref="Identity"/> instance for fluent chaining.</returns>
    public Identity AddPermission(IdentityPermission permission)
    {
        if (Permissions.Where(x => x.Equals(permission)).IsEmpty())
        {
            Permissions.Add(permission);
        }

        return this;
    }

    /// <summary>
    /// Associates a collection of permissions with the identity.
    /// </summary>
    /// <param name="permissions">The collection of permissions to associate.</param>
    /// <returns>The current <see cref="Identity"/> instance for fluent chaining.</returns>
    public Identity AddPermissions(IEnumerable<IdentityPermission> permissions)
    {
        foreach (var permission in permissions)
        {
            AddPermission(permission);
        }

        return this;
    }

    /// <summary>
    /// Associates a role with the identity.
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public Identity AddRole(string role)
    {
        Roles.Add(role);
        return this;
    }

    /// <summary>
    /// Associates all permissions of a specific role with the identity.
    /// </summary>
    /// <param name="role">The role whose permissions should be associated with the identity.</param>
    /// <returns>The current <see cref="Identity"/> instance for fluent chaining.</returns>
    public Identity AddRole(IdentityRole role)
    {
        Roles.Add(role.Name);
        AddPermissions(role.Permissions);
        return this;
    }

    /// <summary>
    /// Associates all permissions of a collection of roles with the identity.
    /// </summary>
    /// <param name="roles">The collection of roles whose permissions should be associated.</param>
    /// <returns>The current <see cref="Identity"/> instance for fluent chaining.</returns>
    public Identity AddRoles(IEnumerable<IdentityRole> roles)
    {
        foreach (var role in roles)
        {
            Roles.Add(role.Name);
            AddRole(role);
        }

        return this;
    }
}

/// <summary>
/// Defines a named list of permissions, representing a specific role in an identity system.
/// </summary>
public class IdentityRole
{
    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the list of permissions associated with this role.
    /// </summary>
    public List<IdentityPermission> Permissions { get; set; }

    /// <summary>
    /// Initializes an empty instance of <see cref="IdentityRole"/>.
    /// </summary>
    [JsonConstructor]
    public IdentityRole()
    {
        Name = string.Empty;
        Permissions = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityRole"/> class with a specified name 
    /// and an optional list of permissions.
    /// </summary>
    /// <param name="name">The name of the role.</param>
    /// <param name="permissions">An optional list of permissions to associate with this role.</param>
    public IdentityRole(string name, IEnumerable<IdentityPermission>? permissions = null)
    {
        Name = name;
        Permissions = permissions?.ToList() ?? new();
    }

    /// <summary>
    /// Adds a permission to the role if it doesn't already exist in the permissions list.
    /// </summary>
    /// <param name="permission">The permission to add.</param>
    /// <returns>Returns the <see cref="IdentityRole"/> instance with the added permission.</returns>
    public IdentityRole AddPermission(IdentityPermission permission)
    {
        if (Permissions.Where(x => x.Equals(permission)).IsEmpty())
        {
            Permissions.Add(permission);
        }

        return this;
    }

    /// <summary>
    /// Adds a collection of permissions to the role. Permissions already present are not duplicated.
    /// </summary>
    /// <param name="permissions">The collection of permissions to add.</param>
    /// <returns>Returns the <see cref="IdentityRole"/> instance with the added permissions.</returns>
    public IdentityRole AddPermissions(IEnumerable<IdentityPermission> permissions)
    {
        foreach (var permission in permissions)
        {
            if (Permissions.Where(x => x.Equals(permission)).IsEmpty())
            {
                Permissions.Add(permission);
            }
        }

        return this;
    }

    /// <summary>
    /// Adds permissions from another <see cref="IdentityRole"/> to the current role. Permissions already present are not duplicated.
    /// </summary>
    /// <param name="role">The role from which permissions are sourced.</param>
    /// <returns>Returns the <see cref="IdentityRole"/> instance with the added permissions.</returns>
    public IdentityRole AddPermissions(IdentityRole role)
    {
        foreach (var permission in role.Permissions)
        {
            if (Permissions.Where(x => x.Equals(permission)).IsEmpty())
            {
                Permissions.Add(permission);
            }
        }

        return this;
    }
}

/// <summary>
/// Represents a specific action with associated permissions, defining how a resource is accessed.
/// </summary>
/// <remarks>
/// An action encapsulates a set of permissions tailored for a specific operation on a resource. <br/>
/// This provides finer-grained access control, allowing for operations such as partial 
/// data reads or specific hard/soft operations. <br/>
/// It becomes especially useful when an identity 
/// is granted permissions to a resource, but access needs to be partial or limited. <br/>
/// Through the action, a specific use case can be mapped to a specific set of permissions.
/// </remarks>
public class IdentityAction
{
    /// <summary>
    /// Gets or sets the domain associated with the action.
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the specific resource under a domain that the action targets.
    /// </summary>
    public string Resource { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the action.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the permissions required for executing this action on the specified resource.
    /// </summary>
    public List<IdentityPermission> RequiredPermissions { get; set; } = new();

    /// <summary>
    /// Initializes an empty instance of <see cref="IdentityAction"/> with all values set to <see cref="string.Empty"/>.
    /// </summary>
    [JsonConstructor]
    public IdentityAction()
    {
        Domain = string.Empty;
        Resource = string.Empty;
        Name = string.Empty;
        RequiredPermissions =  new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityAction"/> class based on an action string.
    /// </summary>
    /// <param name="actionString">The action string in the format "domain:resource:action".</param>
    /// <remarks>
    /// The action string should adhere to the format "domain:resource:action". 
    /// It provides a structured way to quickly define actions and their scope.
    /// </remarks>
    [JsonConstructor]
    public IdentityAction(string actionString, IEnumerable<IdentityPermission>? requiredPermissions = null)
    {
        var split = actionString.Split(':');

        if (split.Length != 3)
        {
            throw new ArgumentException(nameof(actionString));
        }

        Domain = split[0];
        Resource = split[1];
        Name = split[2];
        RequiredPermissions = requiredPermissions?.ToList() ?? new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityAction"/> class using explicit domain, resource, and action name.
    /// </summary>
    /// <param name="domain">The domain associated with the action.</param>
    /// <param name="resource">The specific resource under the domain that the action targets.</param>
    /// <param name="name">The name of the action.</param>
    /// <param name="requiredPermissions">The list of permissions associated with this action.</param>
    public IdentityAction(string domain, string resource, string name, IEnumerable<IdentityPermission>? requiredPermissions = null)
    {
        Domain = domain;
        Resource = resource;
        Name = name;
        RequiredPermissions = requiredPermissions?.ToList() ?? new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityAction"/> class by copying an existing action.
    /// </summary>
    /// <param name="action">The action to be copied.</param>
    public IdentityAction(IdentityAction action)
    {
        Domain = action.Domain;
        Resource = action.Resource;
        Name = action.Name;
        RequiredPermissions = action.RequiredPermissions;
    }

    /// <summary>
    /// Returns a string that represents the current action.
    /// </summary>
    /// <returns>
    /// A string in the format "domain:resource:action".
    /// </returns>
    public override string ToString()
    {
        return $"{Domain}:{Resource}:{Name}";
    }

    /// <summary>
    /// Sets the required permissions for this action and returns the current instance.
    /// </summary>
    /// <param name="requiredPermissions">The list of permissions to be set for this action.</param>
    /// <returns>The current <see cref="IdentityAction"/> instance after updating the required permissions.</returns>
    public IdentityAction SetRequiredPermissions(IEnumerable<IdentityPermission> requiredPermissions)
    {
        RequiredPermissions = RequiredPermissions.ToList();
        return this;
    }

    /// <summary>
    /// Constructs a new <see cref="ResourcePolicy"/> based on the permissions specified for this action.
    /// </summary>
    /// <returns>A new instance of <see cref="ResourcePolicy"/> derived from the required permissions of this action.</returns>
    public virtual ResourcePolicy GetResourcePolicy()
    {
        return new ResourcePolicy(RequiredPermissions);
    }
}

/// <summary>
/// Defines a set of predefined permissions for system use. <br/>
/// This set includes standard CRUD (Create, Read, Update, Delete) operations.
/// </summary>
public static partial class DefinedPermissions
{
    /// <summary>
    /// Represents predefined roles with associated permissions.
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// Represents the system-level permission.
        /// </summary>
        public const string System = "system";

        /// <summary>
        /// Represents the administrative-level permission.
        /// </summary>
        public const string Admin = "admin";

        /// <summary>
        /// Represents the user-level permission.
        /// </summary>
        public const string User = "user";
    }

    /// <summary>
    /// Permission to create entities or resources.
    /// </summary>
    public const string Create = "create";

    /// <summary>
    /// Permission to read or view entities or resources.
    /// </summary>
    public const string Read = "read";

    /// <summary>
    /// Permission to update or modify entities or resources.
    /// </summary>
    public const string Update = "update";

    /// <summary>
    /// Permission to delete entities or resources.
    /// </summary>
    public const string Delete = "delete";

    /// <summary>
    /// Represents a generic write permission. This might include operations like Create or Update.
    /// </summary>
    public const string Write = "write";

    /// <summary>
    /// List of standard CRUD permissions.
    /// </summary>
    public static readonly IReadOnlyList<string> Crud = new[] { Create, Read, Update, Delete };

    /// <summary>
    /// Generates an empty permission that is ignored by the identity system.
    /// </summary>
    public static IdentityPermission GetEmptyPermission()
    {
        return new IdentityPermission();
    }

    /// <summary>
    /// Generates a wildcard system-level permission.
    /// </summary>
    public static IdentityPermission GetSystemPermission()
    {
        return IdentityPermission.WildcardPermission(Roles.System);
    }

    /// <summary>
    /// Generates an administrative-level permission for the specified domain and resource.
    /// </summary>
    /// <param name="domain">The domain to which the permission applies. Uses wildcard by default.</param>
    /// <param name="resource">The resource within the domain. Uses wildcard by default.</param>
    /// <param name="flags">Additional permission flags if needed.</param>
    /// <returns>An <see cref="IdentityPermission"/> object representing administrative access to the specified domain and resource.</returns>
    public static IdentityPermission GetAdminPermission(string domain = IdentityPermission.Wildcard, string resource = IdentityPermission.Wildcard, IEnumerable<IdentityPermission.Flag>? flags = null)
    {
        return new IdentityPermission(domain, resource, Roles.Admin, flags);
    }

    /// <summary>
    /// Generates a user-level permission for the specified domain and resource.
    /// </summary>
    /// <param name="domain">The domain to which the permission applies. Uses wildcard by default.</param>
    /// <param name="resource">The resource within the domain. Uses wildcard by default.</param>
    /// <param name="flags">Additional permission flags if needed.</param>
    /// <returns>An <see cref="IdentityPermission"/> object representing user access to the specified domain and resource.</returns>
    public static IdentityPermission GetUserPermission(string domain = IdentityPermission.Wildcard, string resource = IdentityPermission.Wildcard, IEnumerable<IdentityPermission.Flag>? flags = null)
    {
        return new IdentityPermission(domain, resource, Roles.User, flags);
    }

    /// <summary>
    /// Generates a permission for creating entities or resources within the specified domain and resource.
    /// </summary>
    /// <param name="domain">The domain to which the permission applies. Uses wildcard by default.</param>
    /// <param name="resource">The resource within the domain. Uses wildcard by default.</param>
    /// <param name="flags">Additional permission flags if needed.</param>
    /// <returns>An <see cref="IdentityPermission"/> object representing create access to the specified domain and resource.</returns>
    public static IdentityPermission GetCreatePermission(string domain = IdentityPermission.Wildcard, string resource = IdentityPermission.Wildcard, IEnumerable<IdentityPermission.Flag>? flags = null)
    {
        return new IdentityPermission(domain, resource, Create, flags);
    }

    /// <summary>
    /// Generates a permission for reading entities or resources within the specified domain and resource.
    /// </summary>
    /// <param name="domain">The domain to which the permission applies. Uses wildcard by default.</param>
    /// <param name="resource">The resource within the domain. Uses wildcard by default.</param>
    /// <param name="flags">Additional permission flags if needed.</param>
    /// <returns>An <see cref="IdentityPermission"/> object representing read access to the specified domain and resource.</returns>
    public static IdentityPermission GetReadPermission(string domain = IdentityPermission.Wildcard, string resource = IdentityPermission.Wildcard, IEnumerable<IdentityPermission.Flag>? flags = null)
    {
        return new IdentityPermission(domain, resource, Read, flags);
    }

    /// <summary>
    /// Generates a permission for updating entities or resources within the specified domain and resource.
    /// </summary>
    /// <param name="domain">The domain to which the permission applies. Uses wildcard by default.</param>
    /// <param name="resource">The resource within the domain. Uses wildcard by default.</param>
    /// <param name="flags">Additional permission flags if needed.</param>
    /// <returns>An <see cref="IdentityPermission"/> object representing update access to the specified domain and resource.</returns>
    public static IdentityPermission GetUpdatePermission(string domain = IdentityPermission.Wildcard, string resource = IdentityPermission.Wildcard, IEnumerable<IdentityPermission.Flag>? flags = null)
    {
        return new IdentityPermission(domain, resource, Update, flags);
    }

    /// <summary>
    /// Generates a permission for deleting entities or resources within the specified domain and resource.
    /// </summary>
    /// <param name="domain">The domain to which the permission applies. Uses wildcard by default.</param>
    /// <param name="resource">The resource within the domain. Uses wildcard by default.</param>
    /// <param name="flags">Additional permission flags if needed.</param>
    /// <returns>An <see cref="IdentityPermission"/> object representing delete access to the specified domain and resource.</returns>
    public static IdentityPermission GetDeletePermission(string domain = IdentityPermission.Wildcard, string resource = IdentityPermission.Wildcard, IEnumerable<IdentityPermission.Flag>? flags = null)
    {
        return new IdentityPermission(domain, resource, Delete, flags);
    }
}

/// <summary>
/// Provides a set of predefined roles for managing access control within web applications. <br/>
/// Each role aggregates specific permissions to represent common tasks or responsibilities within the system.
/// </summary>
public static partial class DefinedRoles
{
    const string WildcardString = IdentityPermission.Wildcard;

    /// <summary>
    /// Constructs a wildcard role that grants unrestricted access to all resources and actions within the system. <br/>
    /// WARNING: This role grants unrestricted access to all resources protected by the identity system; <br/>
    /// *use it with caution.*
    /// </summary>
    /// <returns>The wildcard role with unrestricted permissions.</returns>
    public static IdentityRole Wildcard() =>
       new IdentityRole("domain_resource_action_wildcard")
       .AddPermission(new(WildcardString, WildcardString, WildcardString));

    /// <summary>
    /// Constructs a wildcard role with unrestricted access to all resources under a specified domain.
    /// </summary>
    /// <param name="domain">The domain under which all resources are accessible.</param>
    /// <returns>The wildcard role with unrestricted access for the specified domain.</returns>
    public static IdentityRole Wildcard(string domain) =>
        new IdentityRole("resource_action_wildcard")
        .AddPermission(new(domain, WildcardString, WildcardString));

    /// <summary>
    /// Constructs a wildcard role with unrestricted action access for a specified domain and resource.
    /// </summary>
    /// <param name="domain">The domain of the resource.</param>
    /// <param name="resource">The specific resource within the domain.</param>
    /// <returns>The wildcard role with unrestricted action access for the specified domain and resource.</returns>
    public static IdentityRole Wildcard(string domain, string resource) =>
        new IdentityRole("action_wildcard")
        .AddPermission(new(domain, resource, WildcardString));

    //*
    //
    //*

    /// <summary>
    /// Constructs an administrative role that provides general administrative access across all domains and resources.
    /// </summary>
    /// <returns>The administrative role.</returns>
    public static IdentityRole Admin() =>
        new IdentityRole("domain_resource_admin")
        .AddPermission(DefinedPermissions.GetAdminPermission());

    /// <summary>
    /// Constructs an administrative role for a specific domain that provides general administrative access across all resources within that domain.
    /// </summary>
    /// <param name="domain">The domain for which administrative access is granted.</param>
    /// <returns>The administrative role for the specified domain.</returns>
    public static IdentityRole Admin(string domain) =>
        new IdentityRole("resource_admin")
        .AddPermission(DefinedPermissions.GetAdminPermission(domain));

    /// <summary>
    /// Constructs an administrative role for a specific domain and resource.
    /// </summary>
    /// <param name="domain">The domain of the resource.</param>
    /// <param name="resource">The specific resource for which administrative access is granted.</param>
    /// <returns>The administrative role for the specified domain and resource.</returns>
    public static IdentityRole Admin(string domain, string resource) =>
        new IdentityRole("admin")
        .AddPermission(DefinedPermissions.GetAdminPermission(domain, resource));

    /// <summary>
    /// Constructs a read-only role for a specific domain and resource.
    /// </summary>
    /// <param name="domain">The domain of the resource.</param>
    /// <param name="resource">The specific resource for which read-only access is granted.</param>
    /// <returns>The read-only role for the specified domain and resource.</returns>
    public static IdentityRole ReadOnly(string domain, string resource) =>
        new IdentityRole("read_only")
        .AddPermission(DefinedPermissions.GetReadPermission(domain, resource));

    /// <summary>
    /// Constructs a role with both create and read permissions for a specific domain and resource.
    /// </summary>
    /// <param name="domain">The domain of the resource.</param>
    /// <param name="resource">The specific resource for which create and read access is granted.</param>
    /// <returns>The create-read role for the specified domain and resource.</returns>
    public static IdentityRole CreateReadOnly(string domain, string resource) =>
        new IdentityRole("create_read")
        .AddPermission(DefinedPermissions.GetCreatePermission(domain, resource))
        .AddPermission(DefinedPermissions.GetReadPermission(domain, resource));

    /// <summary>
    /// Constructs a role with standard CRUD permissions for a specific domain and resource.
    /// </summary>
    /// <param name="domain">The domain of the resource.</param>
    /// <param name="resource">The specific resource for which CRUD access is granted.</param>
    /// <returns>The CRUD role for the specified domain and resource.</returns>
    public static IdentityRole Crud(string domain, string resource)
    {
        var identity = new IdentityRole("crud");

        foreach (var permission in DefinedPermissions.Crud)
        {
            identity.AddPermission(new($"{domain}:{resource}:{permission}"));
        }

        return identity;
    }
}