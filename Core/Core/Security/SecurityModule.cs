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
    /// Constructs an IdentityPermission with specified domain, resource, and name.
    /// Optionally, a list of flags can be provided.
    /// </summary>
    [JsonConstructor]
    public IdentityPermission(string domain, string resource, string name, List<Flag>? flags = null)
    {
        Domain = domain;
        Resource = resource;
        Name = name;
        Flags = flags ?? new();
    }

    /// <summary>
    /// Constructs an IdentityPermission based on a permission string in the format "domain:resource:name".
    /// Optionally, a list of flags can be provided.
    /// </summary>
    public IdentityPermission(string permission, List<Flag>? flags = null)
    {
        var split = permission.Split(':');

        if (split.Length != 3)
        {
            throw new ArgumentException(nameof(permission));
        }

        Domain = split[0];
        Resource = split[1];
        Name = split[2];
        Flags = flags ?? new();
    }

    /// <summary>
    /// Creates a wildcard permission that signifies a broad or universal permission.
    /// </summary>
    public static IdentityPermission WildcardPermission(string? action = null, List<Flag>? flags = null)
    {
        return new IdentityPermission(Wildcard, Wildcard, action ?? Wildcard, flags);
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
    /// Initializes a new instance of the <see cref="IdentityRole"/> class with a specified name 
    /// and an optional list of permissions.
    /// </summary>
    /// <param name="name">The name of the role.</param>
    /// <param name="permissions">An optional list of permissions to associate with this role.</param>
    [JsonConstructor]
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
    /// Initializes a new instance of the <see cref="Identity"/> class using a unique identifier.
    /// </summary>
    /// <param name="uniqueIdentifier">The unique identifier for the identity.</param>
    [JsonConstructor]
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
    /// Associates all permissions of a specific role with the identity.
    /// </summary>
    /// <param name="role">The role whose permissions should be associated with the identity.</param>
    /// <returns>The current <see cref="Identity"/> instance for fluent chaining.</returns>
    public Identity AddRole(IdentityRole role)
    {
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
            AddRole(role);
        }

        return this;
    }
}

/// <summary>
/// Defines a set of predefined permissions for system use. <br/>
/// This set includes standard CRUD (Create, Read, Update, Delete) operations.
/// </summary>
public static partial class DefinedPermissions
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

    //*
    // spacer
    //*

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
    /// List of permissions that grant both create and read access.
    /// </summary>
    public static readonly IReadOnlyList<string> CreateRead = new[] { Create, Read };

    /// <summary>
    /// List of standard CRUD permissions.
    /// </summary>
    public static readonly IReadOnlyList<string> Crud = new[] { Create, Read, Update, Delete };

    public static IdentityPermission CreateSystemPermission()
    {
        return IdentityPermission.WildcardPermission(System);
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
        .AddPermission(new(WildcardString, WildcardString, DefinedPermissions.Admin));

    /// <summary>
    /// Constructs an administrative role for a specific domain that provides general administrative access across all resources within that domain.
    /// </summary>
    /// <param name="domain">The domain for which administrative access is granted.</param>
    /// <returns>The administrative role for the specified domain.</returns>
    public static IdentityRole Admin(string domain) =>
        new IdentityRole("resource_admin")
        .AddPermission(new(domain, WildcardString, DefinedPermissions.Admin));

    /// <summary>
    /// Constructs an administrative role for a specific domain and resource.
    /// </summary>
    /// <param name="domain">The domain of the resource.</param>
    /// <param name="resource">The specific resource for which administrative access is granted.</param>
    /// <returns>The administrative role for the specified domain and resource.</returns>
    public static IdentityRole Admin(string domain, string resource) =>
        new IdentityRole("admin")
        .AddPermission(new(domain, resource, DefinedPermissions.Admin));

    /// <summary>
    /// Constructs a read-only role for a specific domain and resource.
    /// </summary>
    /// <param name="domain">The domain of the resource.</param>
    /// <param name="resource">The specific resource for which read-only access is granted.</param>
    /// <returns>The read-only role for the specified domain and resource.</returns>
    public static IdentityRole ReadOnly(string domain, string resource) =>
        new IdentityRole("read_only")
        .AddPermission(new(domain, resource, DefinedPermissions.Read));

    /// <summary>
    /// Constructs a role with both create and read permissions for a specific domain and resource.
    /// </summary>
    /// <param name="domain">The domain of the resource.</param>
    /// <param name="resource">The specific resource for which create and read access is granted.</param>
    /// <returns>The create-read role for the specified domain and resource.</returns>
    public static IdentityRole CreateReadOnly(string domain, string resource) =>
        new IdentityRole("create_read")
        .AddPermission(new(domain, resource, DefinedPermissions.Create))
        .AddPermission(new(domain, resource, DefinedPermissions.Read));

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