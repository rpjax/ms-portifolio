using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Core.Patterns;
using ModularSystem.Web.AccessManagement.Extensions;
using ModularSystem.Web.AccessManagement.Services;

namespace ModularSystem.Web.AccessManagement.Authorization;

/// <summary>
/// Provides an authorization service that leverages attributes to determine access control decisions for web resources.
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private IAccessPolicyService AccessPolicyService { get; }
    private IIdentityService IdentityService { get; }

    public AuthorizationService(
        IAccessPolicyService accessPolicyService, 
        IIdentityService identityService)
    {
        AccessPolicyService = accessPolicyService;
        IdentityService = identityService;
    }

    /// <inheritdoc/>
    public async Task<IOperationResult> AuthorizeAsync(HttpContext httpContext)
    {
        var accessPolicy = await AccessPolicyService.GetAccessPolicyAsync(httpContext);
        var identityResult =  IdentityService.GetIdentity(httpContext);

        if (identityResult.IsFailure)
        {
            return identityResult;
        }

        var identity = identityResult.Data;

        if (identity is not null)
        {
            httpContext.SetIdentity(identity);
        }

        // If no identity is present or the access policy fails, return an unauthorized result.
        if (!accessPolicy.Authorize(identity))
        {
            var error = new ErrorBuilder()
                .SetTitle("Unauthorized access.")
                .SetDescription("The user does not have permission to access this resource")
                .AddFlag(ErrorFlags.UserVisible)
                .Build();

            return new OperationResult(error);
        }

        // Return a successful authorization result.
        return new OperationResult();
    }

}
