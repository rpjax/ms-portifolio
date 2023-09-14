using Microsoft.AspNetCore.Http;

namespace ModularSystem.Web;

public class SessionIdentityMiddleware : Middleware
{
    public SessionIdentityMiddleware(RequestDelegate next) : base(next)
    {

    }

    protected override Task BeforeNextAsync(HttpContext context)
    {
        if (context.Items.TryGetValue(SessionController.HTTP_CONTEXT_SESSION_KEY, out var obj))
        {
            if (obj is not ISession)
            {
                throw new InvalidOperationException("Invalid session value was injected in the request.");
            }

            var session = (ISession)obj;
            var identity = session.GetIdentity();

            context.Items.Add(WebController.HTTP_CONTEXT_IDENTITY_KEY, identity);
        }

        return base.BeforeNextAsync(context);
    }
}