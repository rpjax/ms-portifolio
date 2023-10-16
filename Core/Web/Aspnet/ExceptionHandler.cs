using Microsoft.AspNetCore.Http;

namespace ModularSystem.Web;

public abstract class ExceptionHandler
{
    public abstract Task HandleAsync(HttpContext context, Exception exception);
}