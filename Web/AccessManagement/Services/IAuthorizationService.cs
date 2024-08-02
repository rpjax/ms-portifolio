using Microsoft.AspNetCore.Http;
using Aidan.Core.Patterns;

namespace Aidan.Web.AccessManagement.Services;

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
    /// <see cref="IOperationResult"/>, indicating whether the request is authorized, denied, or requires <br/>
    /// further information (e.g., authentication challenge).</returns>
    /// <remarks>
    /// This method is crucial for enforcing security within a web application by ensuring that only <br/>
    /// authorized requests are allowed to proceed. Implementations should consider the request's context, <br/>
    /// such as the user's identity, roles, and any specific resource identifiers present in the request path <br/>
    /// or query strings, to make an informed authorization decision.
    /// </remarks>
    Task<IOperationResult> AuthorizeAsync(HttpContext httpContext);

}
