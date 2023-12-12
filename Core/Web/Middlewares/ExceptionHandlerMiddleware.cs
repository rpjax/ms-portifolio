using Microsoft.AspNetCore.Http;

namespace ModularSystem.Web;

internal class ExceptionHandlerMiddleware : Middleware
{
    public ExceptionHandlerMiddleware(RequestDelegate next) : base(next)
    {

    }

    protected override async Task<Strategy> OnExceptionAsync(HttpContext context, Exception exception)
    {
        await WriteErrorResponseAsync(context, exception);
        return Strategy.Break;
    }

}
