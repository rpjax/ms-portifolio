using ModularSystem.Web.Authentication;
using ModularSystem.Web.Authorization;

namespace ModularSystem.Web;

/// <summary>
/// Defines an interface for a comprehensive access management service that encompasses <br/>
/// both authentication and authorization services. <br/>
/// This interface serves as a central point for managing access control within a web application, <br/>
/// ensuring that both identification and permission verification are handled cohesively.
/// </summary>
public interface IAccessManagementService
{
    /// <summary>
    /// Provides access to the authentication service responsible for identifying users based on provided credentials.
    /// The authentication service is tasked with verifying a user's identity, typically by checking usernames, passwords, tokens, or other authentication factors.
    /// </summary>
    /// <value>
    /// An instance of <see cref="IAuthenticationService"/> that handles user authentication.
    /// </value>
    /// <remarks>
    /// The authentication service is crucial for establishing a user's identity before any authorization can take place. It ensures that users are who they claim to be.
    /// </remarks>
    IAuthenticationService AuthenticationService { get; }

    /// <summary>
    /// Provides access to the authorization service responsible for determining if an authenticated user has permission to access specific resources or perform certain actions.
    /// The authorization service evaluates policies against the authenticated user's roles, permissions, and other criteria to make access control decisions.
    /// </summary>
    /// <value>
    /// An instance of <see cref="IAuthorizationService"/> that handles resource-specific authorization.
    /// </value>
    /// <remarks>
    /// The authorization service is vital for maintaining the security of the application by ensuring that only users with the appropriate permissions can access restricted resources or perform sensitive operations.
    /// </remarks>
    IAuthorizationService AuthorizationService { get; }
}
