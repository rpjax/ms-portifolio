using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Core.Security;
using ModularSystem.Web.Authentication;

namespace ModularSystem.Web;

public class IamAuthenticationMiddleware : Middleware
{
    public static bool ThrowOnExpiredToken { get; set; } = true;

    readonly IIamSystem _iam;

    public IamAuthenticationMiddleware(RequestDelegate next) : base(next)
    {
        if (!DependencyContainer.TryGetInterface<IIamSystem>(out var iam))
        {
            throw new InvalidOperationException("The IamAuthenticationMiddleware requires an implementation of IIamSystem to be registered in the DependencyContainer. Ensure that an appropriate implementation is registered before using this middleware.");
        }

        _iam = iam;
    }

    protected override Task BeforeNextAsync(HttpContext context)
    {
        var token = _iam.AuthenticationProvider.GetToken(context);

        if (token == null)
        {
            return Task.CompletedTask;
        }
        if (token.IsExpired())
        {
            if (ThrowOnExpiredToken)
            {
                throw new AppException("The provided credentials is expired.", ExceptionCode.CredentialsExpired);
            }

            return Task.CompletedTask;
        }

        var identity = _iam.AuthenticationProvider.GetIdentity(token);

        if (identity == null)
        {
            return Task.CompletedTask;
        }

        SetIdentity(context, identity);

        return Task.CompletedTask;
    }

    void SetIdentity(HttpContext context, IIdentity identity)
    {
        context.Items.Add(WebController.HTTP_CONTEXT_IDENTITY_KEY, identity);
    }
}
