using Microsoft.AspNetCore.Http;
using ModularSystem.Core.Patterns;

namespace ModularSystem.Web.AccessManagement.Services;

/// <summary>
/// Represents the interface for the identity service. 
/// <br/>
/// This service is responsible for providing the identity of the current user.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Retrieves the identity asynchronously.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>The operation result containing the identity.</returns>
    IOperationResult<IIdentity?> GetIdentity(HttpContext httpContext);
}
