using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Core.Security;

namespace ModularSystem.Web.Authentication;

/// <summary>
/// Defines an interface for implementing authentication providers in an application. <br/>
/// Authentication providers are responsible for establishing user identity, <br/>
/// potentially based on credentials provided in the request.
/// </summary>
public interface IAuthenticationProvider
{
    /// <summary>
    /// Asynchronously attempts to retrieve the identity of a user based on the provided HTTP context. <br/>
    /// This method processes authentication, which may include checking tokens, credentials, or other forms of identity verification.
    /// </summary>
    /// <param name="httpContext">The HTTP context of the current request, containing all HTTP-specific information.</param>
    /// <returns>
    /// A task representing the asynchronous operation, resulting in an <see cref="OperationResult{IIdentity}"/>. <br/>
    /// The result provides the outcome of the authentication attempt, including a potentially authenticated user's identity <br/>
    /// or error information if authentication could not be conclusively processed.
    /// </returns>
    /// <remarks>
    /// This method returns an operation result to convey the authentication attempt's outcome. <br/>
    /// A successful operation with null data typically represents the absence of valid credentials. <br/>
    /// For example, when provided credentials are expired or otherwise invalid, yet the request format and processing are correct. <br/>
    /// Conversely, a failed operation indicates unexpected issues, such as deserialization errors, invalid formats, or other system-level failures.
    /// </remarks>
    Task<OperationResult<IIdentity>> TryGetIdentityAsync(HttpContext httpContext);
}
