using Microsoft.AspNetCore.Http;
using ModularSystem.Core;

namespace ModularSystem.Web;

public abstract class SessionController : WebController
{
    public const string HTTP_CONTEXT_SESSION_KEY = "__session_injection";

    protected ISession session => GetSession();

    protected virtual ISession GetSession()
    {
        if (HttpContext.Items.TryGetValue(HTTP_CONTEXT_SESSION_KEY, out object? value))
        {
            var session = value?.TryTypeCast<ISession>();

            if (session == null)
            {
                throw new AppException("Could not get the 'Session' object from 'HttpContext.Items'.", ExceptionCode.Internal);
            }

            return session;
        }

        throw new AppException("Could not get the 'Session' object from 'HttpContext.Items'.", ExceptionCode.Internal);
    }

    protected virtual T GetSession<T>() where T : class, ISession
    {
        if (HttpContext.Items.TryGetValue(HTTP_CONTEXT_SESSION_KEY, out object? value))
        {
            var session = value?.TryTypeCast<T>();

            if (session == null)
            {
                throw new AppException("Could not get the 'Session' object from 'HttpContext.Items'.", ExceptionCode.Internal);
            }

            return session;
        }

        throw new AppException("Could not get the 'Session' object from 'HttpContext.Items'.", ExceptionCode.Internal);
    }
}
