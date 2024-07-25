using Microsoft.AspNetCore.Http;
using ModularSystem.Core.AccessManagement;

namespace ModularSystem.Web.Authentication;

/// <summary>
/// Defines an interface for a service that handles the authentication process within a web application. <br/>
/// This service is responsible for determining the identity of a user based on the given HTTP context, <br/>
/// which typically involves extracting and validating credentials from the request.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Attempts to authenticate a user and retrieve their identity based on the provided HTTP context. 
    /// This method examines the HTTP request for authentication data (such as tokens, cookies, or headers) 
    /// and uses that data to authenticate the user.
    /// </summary>
    /// <param name="httpContext">The HTTP context of the current request, containing all necessary 
    /// information for authentication, including headers, cookies, and the request path.</param>
    /// <returns>
    /// A task representing the asynchronous operation of the authentication process. Upon completion, 
    /// the task yields an <see cref="IIdentity"/> object representing the authenticated user's identity if 
    /// authentication is successful; otherwise, null if authentication fails or no user could be authenticated.
    /// </returns>
    /// <remarks>
    /// Example Usage:
    /// An implementation could look for a JWT (JSON Web Token) in the Authorization header of the 
    /// <paramref name="httpContext"/>. If present, the token is validated, and if valid, an IIdentity 
    /// for the user is constructed and returned. If the token is missing, invalid, or expired, the method 
    /// would return null, indicating that the user could not be authenticated.
    /// </remarks>
    Task<IIdentity?> TryGetIdentityAsync(HttpContext httpContext);
}
