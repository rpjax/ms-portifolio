using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Core.AccessManagement;

namespace ModularSystem.Web.Authorization;

/// <summary>
/// Defines an interface for managing and retrieving authorization policies specific to web resources. <br/>
/// Implementations of this interface should determine the applicable authorization policy based on the provided HTTP context, <br/>
/// allowing for resource-specific access control decisions.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Asynchronously evaluates the authorization policy for a specific HTTP context, determining if the <br/>
    /// request is authorized to access the targeted resource. <br/>
    /// </summary>
    /// <param name="httpContext">The HTTP context for the current request. This context contains all <br/>
    /// the request information, including headers, cookies, and the requested path, which are used to <br/>
    /// evaluate the applicable authorization policy.</param>
    /// <returns>A task that represents the asynchronous evaluation operation. The task result is an <br/>
    /// <see cref="AuthorizationResult"/>, indicating whether the request is authorized, denied, or requires <br/>
    /// further information (e.g., authentication challenge).</returns>
    /// <remarks>
    /// This method is crucial for enforcing security within a web application by ensuring that only <br/>
    /// authorized requests are allowed to proceed. Implementations should consider the request's context, <br/>
    /// such as the user's identity, roles, and any specific resource identifiers present in the request path <br/>
    /// or query strings, to make an informed authorization decision.
    /// </remarks>
    Task<AuthorizationResult> AuthorizeAsync(HttpContext httpContext, IIdentity? identity);
}

/// <summary>
/// Represents the outcome of an authorization attempt, encapsulating details about the authorization success or failure.
/// </summary>
public class AuthorizationResult : OperationResult
{
    /// <summary>
    /// Optionally provides a reason for authorization failure, offering insights into why a request was denied access.
    /// </summary>
    public string? AuthorizationFailureReason { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationResult"/> class, allowing specification of both the authorization outcome and a failure reason.
    /// </summary>
    /// <param name="isAuthorized">Indicates whether the authorization attempt was successful. A value of <c>true</c> indicates success, while <c>false</c> indicates failure.</param>
    /// <param name="authorizationFailureReason">Provides a detailed reason for the authorization failure. This is useful for logging, debugging, or providing feedback to the end-user. This parameter is ignored if <paramref name="isAuthorized"/> is <c>true</c>.</param>
    /// <remarks>
    /// This constructor enhances flexibility by allowing direct specification of the authorization outcome and an optional reason for failure. It's particularly useful in scenarios where the decision logic is complex or involves multiple steps, and you need to provide specific feedback based on the outcome.
    /// </remarks>
    public AuthorizationResult(bool isAuthorized, string? authorizationFailureReason = null)
    {
        IsSuccess = isAuthorized;
        AuthorizationFailureReason = isAuthorized ? null : authorizationFailureReason;
    }

}
