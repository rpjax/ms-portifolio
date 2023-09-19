using ModularSystem.Web.Authentication;
using ModularSystem.Web.Authorization;

namespace ModularSystem.Web;

/// <summary>
/// Defines an interface for Identity and Access Management (IAM) systems within the web context. <br/>
/// It encompasses both authentication (verifying who a user is) and authorization (determining what a user can do).
/// </summary>
public interface IIamSystem
{
    /// <summary>
    /// Gets the instance of <see cref="IAuthenticationProvider"/> which is responsible for authenticating and 
    /// verifying the identity of users within the system.
    /// </summary>
    /// <value>
    /// The authentication provider.
    /// </value>
    IAuthenticationProvider AuthenticationProvider { get; }

    /// <summary>
    /// Gets the instance of <see cref="IAuthorizationProvider"/> which is responsible for managing and determining 
    /// the permissions and access rights of authenticated users.
    /// </summary>
    /// <value>
    /// The authorization provider.
    /// </value>
    IAuthorizationProvider AuthorizationProvider { get; }
}
