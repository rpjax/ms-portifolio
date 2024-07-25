using ModularSystem.Core;
using ModularSystem.Core.Logging;

namespace ModularSystem.Web;

internal class ExceptionHandlerMiddleware : Middleware
{
    private IErrorLogger? Logger { get; }

    public ExceptionHandlerMiddleware(RequestDelegate next, IErrorLogger? logger) : base(next)
    {
        Logger = logger;
    }

    protected override async Task<Strategy> OnExceptionAsync(HttpContext context, Exception exception)
    {
        if(Logger is not null)
        {
            Logger.Log(Error.FromException(exception));
        }

        await WriteExceptionResponseAsync(context, exception);
        return Strategy.Break;
    }

}
