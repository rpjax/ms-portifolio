using Microsoft.AspNetCore.Http;
using Aidan.Web.AccessManagement.Attributes;
using Aidan.Web.AccessManagement.Services;

namespace Aidan.Web.AccessManagement.Authorization;

/// <summary>
/// Represents a service for retrieving access policies based on attributes.
/// </summary>
public class AttributeAccessPolicyService : IAccessPolicyService
{
    /// <summary>
    /// Retrieves the access policy for the specified HTTP context.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>The access policy.</returns>
    public Task<IAccessPolicy> GetAccessPolicyAsync(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        var metadata = endpoint?.Metadata;
        var attribute = metadata?.GetMetadata<AccessManagementAttribute>();
        var accessPolicy = attribute?.GetAccessPolicy() ?? new AccessPolicy();
        return Task.FromResult(accessPolicy);
    }
}
