using Microsoft.AspNetCore.Http;
using ModularSystem.Core.Security;

namespace ModularSystem.Web.Authorization;

/// <summary>
/// Defines an interface for managing and retrieving authorization policies specific to resources. <br/>
/// Implementations of this interface would determine the applicable policy based on the provided HTTP context.
/// </summary>
public interface IAuthorizationProvider
{
    /// <summary>
    /// Asynchronously retrieves the resource-specific authorization policy associated with the specified HTTP context.
    /// </summary>
    /// <param name="httpContext">
    /// The HTTP context based on which the appropriate resource policy is determined.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. Upon its completion, the task yields the <see cref="IResourcePolicy"/> <br/>
    /// corresponding to the given HTTP context or null if no matching policy is found.
    /// </returns>
    Task<IResourcePolicy?> GetResourcePolicyAsync(HttpContext httpContext);
}
