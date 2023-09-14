namespace ModularSystem.Web.Authentication;

/// <summary>
/// Represents an interface for Identity and Access Management (IAM) systems, providing authentication and authorization functionalities.
/// </summary>
public interface IIamSystem
{
    /// <summary>
    /// Gets the authentication provider responsible for verifying the identity of users.
    /// </summary>
    IAuthenticationProvider AuthenticationProvider { get; }

    /// <summary>
    /// Gets the authorization provider responsible for determining user permissions and access rights.
    /// </summary>
    IAuthorizationProvider AuthorizationProvider { get; }
}
