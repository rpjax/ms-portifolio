using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ModularSystem.Core;
using ModularSystem.Core.Logging;
using System.Text;

namespace ModularSystem.Web;

/// <summary>
/// Provides a base implementation for creating custom middleware components in an ASP.NET Core application.
/// </summary>
public abstract class Middleware
{
    /// <summary>
    /// Gets the next middleware in the request processing pipeline. <br/>
    /// This delegate is invoked after the current middleware completes its processing.
    /// </summary>
    protected RequestDelegate Next { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Middleware"/> class with the specified next delegate in the pipeline.
    /// </summary>
    /// <param name="next">The next delegate in the request pipeline.</param>
    public Middleware(RequestDelegate next)
    {
        Next = next;
    }

    /// <summary>
    /// Invokes the middleware with the given context. <br/>
    /// If <see cref="BeforeNextAsync"/> returns true, the next middleware in the pipeline will be executed. <br/>
    /// After that, if <see cref="AfterNextAsync"/> returns true, reserved future additions (if any) will be executed.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (await BeforeNextAsync(context))
            {
                await Next(context);
            }

            if(await AfterNextAsync(context))
            {
                // reserved for future additions...
            }
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    /// <summary>
    /// Method to be executed before the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task that, when executed, will return true if the next middleware should be executed, otherwise false.</returns>
    protected virtual Task<bool> BeforeNextAsync(HttpContext context)
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// Method to be executed after the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task that, when executed, will return true if reserved future additions should be executed, otherwise false.</returns>
    protected virtual Task<bool> AfterNextAsync(HttpContext context)
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// Sends an HTTP response with no content.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    protected void WriteNoContentResponse(HttpContext context)
    {
        context.Response.Clear();
        context.Response.StatusCode = 204;
    }

    /// <summary>
    /// Sends an HTTP response with the specified content type and data.
    /// </summary>
    protected async Task WriteTextResponseAsync(HttpContext context, int statusCode, string contentType, string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = contentType;
        context.Response.ContentLength = bytes.Length;

        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Sends a JSON response with the specified status code and data.
    /// </summary>
    protected Task WriteJsonResponseAsync(HttpContext context, int statusCode, string data)
    {
        return WriteTextResponseAsync(context, statusCode, "application/json", data);
    }

    /// <summary>
    /// Sends an HTML response with the specified status code and data.
    /// </summary>
    protected Task WriteHtmlResponseAsync(HttpContext context, int statusCode, string html)
    {
        return WriteTextResponseAsync(context, statusCode, "text/html", html);
    }

    /// <summary>
    /// Retrieves the value of a specified cookie from the current HTTP request.
    /// </summary>
    protected string? GetCookie(HttpContext context, string name)
    {
        if (context.Request.Cookies.TryGetValue(name, out string? cookie))
        {
            return cookie;
        }

        return null;
    }

    /// <summary>
    /// Retrieves the value of a specified query parameter from the current HTTP request.
    /// </summary>
    protected string? GetQueryParam(HttpContext context, string name)
    {
        if (context.Request.Query.TryGetValue(name, out StringValues stringValue))
        {
            return stringValue.FirstOrDefault();
        }

        return null;
    }

    /// <summary>
    /// Sets a cookie with the specified key and value in the HTTP response.
    /// </summary>
    protected void SetCookie(HttpContext context, string key, string value)
    {
        var options = new CookieOptions() { Expires = TimeProvider.Now() };
        context.Response.Cookies.Append(key, value, options);
    }

    /// <summary>
    /// Sets a cookie with the specified key, value, and expiration date in the HTTP response.
    /// </summary>
    protected void SetCookie(HttpContext context, string key, string value, DateTime expires)
    {
        var options = new CookieOptions() { Expires = expires };
        context.Response.Cookies.Append(key, value, options);
    }

    /// <summary>
    /// Sets a cookie with the specified options in the HTTP response.
    /// </summary>
    protected void SetCookie(HttpContext context, string key, string value, CookieOptions options)
    {
        context.Response.Cookies.Append(key, value, options);
    }

    /// <summary>
    /// Deletes the specified cookie from the HTTP response.
    /// </summary>
    protected void DeleteCookie(HttpContext context, string cookieName)
    {
        context.Response.Cookies.Delete(cookieName);
    }

    /// <summary>
    /// Method that is called when an exception occurs during the processing of the middleware.
    /// </summary>
    protected virtual Task OnException(HttpContext context, Exception exception)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles exceptions that may occur during the processing of the middleware.
    /// </summary>
    protected virtual async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var appException = exception.ToAppException();
        var statusCode = AppExceptionPresenter.GetStatusCodeFrom(appException);
        var json = AppExceptionPresenter.ToJson(appException);

        ExceptionLogger.Log(appException);
        await WriteJsonResponseAsync(context, statusCode, json);
    }
}
