using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Core.AccessManagement;
using ModularSystem.Web.Attributes;

namespace ModularSystem.Web.Authorization;

/// <summary>
/// Provides an authorization service that leverages attributes to determine access control decisions for web resources.
/// </summary>
public class AttributeAuthorizationService : IAuthorizationService
{
    private IAsyncStrategy<string, AccessPolicy> Strategy { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeAuthorizationService"/> with a strategy for retrieving access policies.
    /// </summary>
    /// <param name="strategy">The strategy to use for determining access policies based on routes.</param>
    public AttributeAuthorizationService(IAsyncStrategy<string, AccessPolicy> strategy)
    {
        Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy), "Strategy cannot be null.");
    }

    /// <summary>
    /// Authorizes an HTTP request based on attributes defined on endpoints and the current identity's permissions.
    /// </summary>
    /// <param name="httpContext">The HTTP context for the current request.</param>
    /// <param name="identity">The identity of the user making the request, or null if the user is not authenticated.</param>
    /// <returns>A task that represents the asynchronous authorization operation. The task result is an <see cref="AuthorizationResult"/>.</returns>
    public async Task<AuthorizationResult> AuthorizeAsync(HttpContext httpContext, IIdentity? identity)
    {
        // Check for anonymous access.
        var anonymousAttribute = TryGetAnonymousAttribute(httpContext);
        if (anonymousAttribute != null)
        {
            return new AuthorizationResult(true);
        }

        // Check for authorization requirement.
        var authorizeAttribute = TryGetAuthorizeAttribute(httpContext);
        if (authorizeAttribute == null)
        {
            return new AuthorizationResult(true);
        }

        // Determine the access policy based on the request route.
        var route = GetRoute(httpContext);
        var accessPolicy = await Strategy.ExecuteAsync(route);

        // If no identity is present or the access policy fails, return an unauthorized result.
        if (identity == null || !accessPolicy.Authorize(identity))
        {
            return new AuthorizationResult(false);
        }

        // Return a successful authorization result.
        return new AuthorizationResult(true);
    }

    private AccessManagementAttribute? TryGetAuthorizeAttribute(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        return endpoint?.Metadata.GetMetadata<AccessManagementAttribute>();
    }

    private AnonymousActionAttribute? TryGetAnonymousAttribute(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        return endpoint?.Metadata.GetMetadata<AnonymousActionAttribute>();
    }

    /// <summary>
    /// Extracts the route from the HTTP context, which is used to determine the applicable access policy.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>The route as a string.</returns>
    private string GetRoute(HttpContext context)
    {
        return context.Request.Path;
    }
}
