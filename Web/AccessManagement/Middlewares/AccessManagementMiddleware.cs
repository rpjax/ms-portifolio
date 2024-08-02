using Microsoft.AspNetCore.Http;
using Aidan.Core;
using Aidan.Web.AccessManagement.Services;

namespace Aidan.Web.AccessManagement.Middlewares;

/// <summary>
/// Middleware for managing access control within the application, integrating authentication and authorization services.
/// </summary>
public class AccessManagementMiddleware : Middleware
{
    private IAuthorizationService AuthorizationService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessManagementMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware delegate in the request pipeline.</param>
    /// <param name="service">The access management service used for authentication and authorization.</param>
    public AccessManagementMiddleware(
        RequestDelegate next,
        IAuthorizationService authorizationService)
        : base(next)
    {
        AuthorizationService = authorizationService;
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
        var result = await AuthorizationService.AuthorizeAsync(context);

        if (result.IsSuccess)
        {
            return Strategy.Continue;
        }

        await WriteUnauthorizedResponseAsync(context, result.Errors);
        return Strategy.Break;
    }

    /// <summary>
    /// Writes an unauthorized response to the client, including a message indicating the failure reason.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    private async Task WriteUnauthorizedResponseAsync(HttpContext context, IEnumerable<Error> errors)
    {
        var title = "Unauthorized access.";
        var detail = "The user does not have permission to access this resource.";

        await context.WriteProblemResponseAsync(
            statusCode: 401,
            title: title,
            detail: detail,
            errors: errors);
    }

}
