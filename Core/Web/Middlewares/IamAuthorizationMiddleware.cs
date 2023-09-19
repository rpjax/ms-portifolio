using Microsoft.AspNetCore.Http;
using ModularSystem.Core;

namespace ModularSystem.Web;

/// <summary>
/// Middleware for handling IAM (Identity and Access Management) based authorization. <br/>
/// It uses the registered <see cref="IIamSystem"/>  implementation to authorize the incoming requests.
/// </summary>
public class IamAuthorizationMiddleware : Middleware
{
    /// <summary>
    /// The IIamSystem instance retrieved from the DependencyContainer.
    /// </summary>
    private IIamSystem Iam { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IamAuthorizationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <exception cref="InvalidOperationException">Thrown when an appropriate IIamSystem implementation is not found in the DependencyContainer.</exception>
    public IamAuthorizationMiddleware(RequestDelegate next) : base(next)
    {
        if (!DependencyContainer.TryGetInterface<IIamSystem>(out var iam))
        {
            throw new InvalidOperationException("The IamAuthenticationMiddleware requires an implementation of IIamSystem to be registered in the DependencyContainer. Ensure that an appropriate implementation is registered before using this middleware.");
        }

        Iam = iam;
    }

    /// <summary>
    /// Executes before the next middleware. If a resource policy is available for the incoming request, <br/>
    /// checks the user's identity against the policy. If the user is not authorized, an exception is thrown.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A value indicating whether the middleware pipeline should continue.</returns>
    /// <exception cref="AppException">Thrown when the authenticated user lacks the required permissions to execute the operation.</exception>
    protected override async Task<bool> BeforeNextAsync(HttpContext context)
    {
        var resourcePolicy = await Iam.AuthorizationProvider.GetResourcePolicyAsync(context);

        if (resourcePolicy == null)
        {
            return true;
        }

        var isAuthorized = await resourcePolicy.TryAuthorizeAsync(context.TryGetIdentity());

        if (!isAuthorized)
        {
            throw new AppException("The authenticated user lacks the required permissions to execute this operation. Ensure you have the right privileges.", ExceptionCode.Unauthorized);
        }

        return true;
    }
}
