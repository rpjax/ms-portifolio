using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Web.AccessManagement;
using ModularSystem.Web.Attributes;

namespace ModularSystem.Web;

/// <summary>
/// Middleware for managing access control within the application, integrating authentication and authorization services.
/// </summary>
public class AccessManagementMiddleware : Middleware
{
    private IAccessManagementService Service { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessManagementMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware delegate in the request pipeline.</param>
    /// <param name="service">The access management service used for authentication and authorization.</param>
    public AccessManagementMiddleware(RequestDelegate next, IAccessManagementService service) : base(next)
    {
        Service = service;
    }

    /// <summary>
    /// Invoked by the runtime to process each request through the middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A <see cref="Middleware.Strategy"/> indicating whether to continue the request pipeline or break.</returns>
    /// <remarks>
    /// This method checks for the presence of <see cref="AnonymousActionAttribute"/> and <see cref="AccessManagementAttribute"/>
    /// on the current endpoint. If an anonymous attribute is found, the request continues without further checks.
    /// If an authorize attribute is found, the middleware attempts to authenticate and authorize the request based on the provided identity.
    /// Unauthorized requests result in a 401 response.
    /// </remarks>
    protected override async Task<Strategy> BeforeNextAsync(HttpContext context)
    {
        //var anonymousAttribute = TryGetAnonymousAttribute(context);
        //var authorizeAttribute = TryGetAuthorizeAttribute(context);

        //// If the endpoint allows anonymous access or doesn't require authorization, continue the pipeline.
        //if (anonymousAttribute != null || authorizeAttribute == null)
        //{
        //    return Strategy.Continue;
        //}

        var identity = await Service.AuthenticationService.TryGetIdentityAsync(context);
        var authorizationResult = await Service.AuthorizationService.AuthorizeAsync(context, identity);

        if (authorizationResult.IsFailure)
        {
            await WriteUnauthorizedResponseAsync(context);
            return Strategy.Break;
        }

        if (identity != null)
        {
            context.SetIdentity(identity);
        }

        return Strategy.Continue;
    }

    /// <summary>
    /// Writes an unauthorized response to the client, including a message indicating the failure reason.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    private async Task WriteUnauthorizedResponseAsync(HttpContext context)
    {
        var message = "The authenticated user lacks the required permissions to execute this operation. Ensure you have the right privileges.";
        var error = new Error(message).AddFlags(ErrorFlags.Public);
        var result = new OperationResult(error);

        await context.WriteOperationResponseAsync(result, 401);
    }

    /// <summary>
    /// Attempts to retrieve an <see cref="AccessManagementAttribute"/> from the current endpoint's metadata.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>The found <see cref="AccessManagementAttribute"/>, or null if not present.</returns>
    private AccessManagementAttribute? TryGetAuthorizeAttribute(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        return endpoint?.Metadata.GetMetadata<AccessManagementAttribute>();
    }

    /// <summary>
    /// Attempts to retrieve an <see cref="AnonymousActionAttribute"/> from the current endpoint's metadata.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>The found <see cref="AnonymousActionAttribute"/>, or null if not present.</returns>
    private AnonymousActionAttribute? TryGetAnonymousAttribute(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        return endpoint?.Metadata.GetMetadata<AnonymousActionAttribute>();
    }
}
