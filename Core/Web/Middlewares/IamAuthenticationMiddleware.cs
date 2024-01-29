using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Core.Security;

namespace ModularSystem.Web;

/// <summary>
/// Middleware that integrates the Identity and Access Management (IAM) system for user authentication within the application pipeline.<br/> 
/// It intercepts incoming requests and attempts to extract a user's identity from them. <br/> 
/// If successfully authenticated, this identity is made available within the request's context for subsequent components. <br/> 
/// This middleware relies on the registered implementation of the <see cref="IIamService"/> interface to provide the authentication logic and mechanisms.
/// </summary>
public class IamAuthenticationMiddleware : Middleware
{
    /// <summary>
    /// The IAM system that handles authentication-related operations.
    /// </summary>
    private IIamService Iam { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IamAuthenticationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    public IamAuthenticationMiddleware(RequestDelegate next) : base(next)
    {
        if (!DependencyContainer.TryGetInterface<IIamService>(out var iam))
        {
            throw new InvalidOperationException("The IamAuthenticationMiddleware requires an implementation of IIamSystem to be registered in the DependencyContainer. Ensure that an appropriate implementation is registered before using this middleware.");
        }

        Iam = iam;
    }

    /// <summary>
    /// Executes tasks before the next middleware in the pipeline. In this context, it retrieves the identity from the HTTP context and, if valid, injects it for further processing.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a value indicating if the next middleware should be invoked.</returns>
    protected override async Task<Strategy> BeforeNextAsync(HttpContext context)
    {
        var identity = await context.TryGetIdentityAsync();

        if (identity != null)
        {
            InjectIdentity(context, identity);
        }

        return Strategy.Continue;
    }

    /// <summary>
    /// Injects the validated identity into the HTTP context for further use by subsequent middlewares or request handlers.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="identity">The validated identity to inject.</param>
    private void InjectIdentity(HttpContext context, IIdentity identity)
    {
        context.Items.Add(WebController.HttpContextIdentityKey, identity);
    }
}
