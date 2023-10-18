using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ModularSystem.Core;
using System.Text;

namespace ModularSystem.Web;

/// <summary>
/// Provides a foundational structure for creating custom middleware components in an ASP.NET Core application.
/// </summary>
public abstract class Middleware
{
    /// <summary>
    /// Enumerates strategies for middleware execution flow.
    /// </summary>
    public enum Strategy
    {
        /// <summary>
        /// Continue executing the next middleware in the pipeline.
        /// </summary>
        Continue,

        /// <summary>
        /// Break the middleware execution and do not proceed to the next middleware.
        /// </summary>
        Break
    }

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
    /// Invokes the middleware using the provided context.
    /// Depending on the result of <see cref="BeforeNextAsync"/>, the subsequent middleware in the pipeline might be executed.
    /// Post that, <see cref="AfterNextAsync"/> is executed.
    /// </summary>
    /// <param name="context">The prevailing HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task InvokeAsync(HttpContext context)
    {
        try
        {
            switch (await BeforeNextAsync(context))
            {
                case Strategy.Continue:
                    await Next(context);
                    break;
                case Strategy.Break:
                    return;
            }

            await AfterNextAsync(context);    
        }
        catch (Exception e)
        {
            switch (await OnExceptionAsync(context, e))
            {
                case Strategy.Continue:
                    throw;
                case Strategy.Break:
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    /// <summary>
    /// Method to be executed before the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task that, when executed, will return true if the next middleware should be executed, otherwise false.</returns>
    protected virtual Task<Strategy> BeforeNextAsync(HttpContext context)
    {
        return Task.FromResult(Strategy.Continue);
    }

    /// <summary>
    /// Method to be executed after the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task that, when executed, will return true if reserved future additions should be executed, otherwise false.</returns>
    protected virtual Task AfterNextAsync(HttpContext context)
    {
        return Task.CompletedTask;
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
    /// Writes an error response to the client based on the provided exception.
    /// </summary>
    /// <param name="context">The prevailing HTTP context.</param>
    /// <param name="exception">The exception to be processed and presented to the client.</param>
    /// <returns>A task representing the asynchronous operation of writing the error response.</returns>
    /// <remarks>
    /// This method first converts the given exception into a standardized application exception using <see cref="Exception.ToAppException"/>.
    /// It then determines the appropriate HTTP status code and formats the exception as a JSON response.
    /// If the exception represents an internal error, it will be logged for further analysis.
    /// </remarks>
    protected Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        return context.WriteErrorResponseAsync(exception);
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
    /// Handles exceptions that occur during the processing of the middleware.
    /// This method provides a mechanism to handle exceptions in a centralized manner, 
    /// allowing for custom error handling, logging, or response modification.
    /// By default, this method re-throws the exception, allowing it to be caught by 
    /// any subsequent error-handling middleware in the pipeline. Override this method 
    /// in derived classes to implement custom exception handling behavior.
    /// </summary>
    /// <param name="context">The prevailing HTTP context.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task<Strategy> OnExceptionAsync(HttpContext context, Exception exception)
    {
        return Task.FromResult(Strategy.Continue); 
    }

}
