using Microsoft.AspNetCore.Http;
using ModularSystem.Core.Logging;
using ModularSystem.Core;
using ModularSystem.Web.Expressions;

namespace ModularSystem.Web;

internal class ExceptionHandlerMiddleware : Middleware
{
    public ExceptionHandlerMiddleware(RequestDelegate next) : base(next)
    {
        
    }

    protected override async Task OnExceptionAsync(HttpContext context, Exception exception)
    {
        var appException = exception.ToAppException();
        var statusCode = AppExceptionPresenter.GetStatusCodeFrom(appException);
        var json = AppExceptionPresenter.ToJson(appException);

        ExceptionLogger.Log(appException);
        await WriteJsonResponseAsync(context, statusCode, json);
    }

}
