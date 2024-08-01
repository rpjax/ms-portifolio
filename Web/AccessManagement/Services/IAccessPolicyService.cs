using Microsoft.AspNetCore.Http;

namespace ModularSystem.Web.AccessManagement.Services;

/// <summary>
/// Represents a service for accessing access policies.
/// </summary>
public interface IAccessPolicyService
{
    /// <summary>
    /// Retrieves the access policy for the specified HTTP context.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>The access policy.</returns>
    Task<IAccessPolicy> GetAccessPolicyAsync(HttpContext httpContext);
}
