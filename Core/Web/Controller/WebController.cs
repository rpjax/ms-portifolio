using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ModularSystem.Core;
using ModularSystem.Core.Logging;
using ModularSystem.Core.Security;

namespace ModularSystem.Web;

/// <summary>
/// Represents a base web controller that provides utility methods and exception handling mechanisms.
/// </summary>
public abstract class WebController : ControllerBase
{
    /// <summary>
    /// Key used for storing and retrieving the identity from the HttpContext.Items dictionary.
    /// </summary>
    public const string HTTP_CONTEXT_IDENTITY_KEY = "__identity_injection";

    /// <summary>
    /// Determines whether to log exceptions or not.
    /// </summary>
    protected bool EnableExceptionLogging { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebController"/> class.
    /// </summary>
    protected WebController()
    {

    }

    /// <summary>
    /// Handles exceptions and returns an appropriate response.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <returns>An IActionResult representing the exception response.</returns>
    protected virtual IActionResult HandleException(Exception exception)
    {
        OnException(exception);

        var appException = exception.ToAppException();

        if (EnableExceptionLogging)
        {
            LogException(appException);
        }

        var json = AppExceptionPresenter.ToJson(appException);
        var statusCode = AppExceptionPresenter.GetStatusCodeFrom(appException);

        HttpContext.Response.ContentType = "application/json";
        HttpContext.Response.StatusCode = statusCode;

        return Content(json, "application/json");
    }

    /// <summary>
    /// Event method that is called upon an exception.
    /// </summary>
    protected virtual void OnException(Exception exception)
    {

    }

    /// <summary>
    /// Logs an application exception.
    /// </summary>
    /// <param name="e">The application exception to log.</param>
    protected virtual void LogException(AppException e)
    {
        if (e.Code == ExceptionCode.Internal)
        {
            ExceptionLogger.Log(e);
        }
    }

    /// <summary>
    /// Retrieves a cookie value by its name.
    /// </summary>
    /// <param name="name">The name of the cookie.</param>
    /// <returns>The cookie value if found; otherwise, null.</returns>
    protected string? GetCookie(string name)
    {
        if (Request.Cookies.TryGetValue(name, out string? cookie))
        {
            return cookie;
        }

        return null;
    }

    /// <summary>
    /// Retrieves a query parameter value by its name.
    /// </summary>
    /// <param name="name">The name of the query parameter.</param>
    protected string? GetQueryParam(string name)
    {
        if (Request.Query.TryGetValue(name, out StringValues stringValue))
        {
            return stringValue.FirstOrDefault();

        }

        return null;
    }

    /// <summary>
    /// Sets a cookie with a specific key, value, and expiration date.
    /// </summary>
    /// <param name="key">The cookie name.</param>
    /// <param name="value">The cookie value.</param>
    /// <param name="expires">The expiration date and time for the cookie.</param>
    protected void SetCookie(string key, string value, DateTime expires)
    {
        var options = new CookieOptions() { Expires = expires };
        Response.Cookies.Append(key, value, options);
    }

    /// <summary>
    /// Sets a cookie with a specific key, value, and options.
    /// </summary>
    /// <param name="key">The cookie name.</param>
    /// <param name="value">The cookie value.</param>
    /// <param name="options">Options for the cookie.</param>
    protected void SetCookie(string key, string value, CookieOptions options)
    {
        Response.Cookies.Append(key, value, options);
    }

    /// <summary>
    /// Deletes a cookie by its name.
    /// </summary>
    /// <param name="key">The name of the cookie to delete.</param>
    protected void DeleteCookie(string key)
    {
        Response.Cookies.Delete(key);
    }

    /// <summary>
    /// Attempts to retrieve the identity object from the HttpContext.Items dictionary.
    /// </summary>
    /// <returns>An IIdentity object if found; otherwise, null.</returns>
    protected virtual IIdentity? TryGetIdentity()
    {
        if (HttpContext.Items.TryGetValue(HTTP_CONTEXT_IDENTITY_KEY, out object? value))
        {
            var identity = value?.TryTypeCast<IIdentity>();

            if (identity == null)
            {
                return null;
            }

            return identity;
        }

        return null;
    }

    /// <summary>
    /// Retrieves the identity object from the HttpContext.Items dictionary. Throws an exception if not found.
    /// </summary>
    /// <returns>An IIdentity object.</returns>
    /// <exception cref="AppException">Thrown when the identity object cannot be found in the HttpContext.Items dictionary.</exception>
    protected virtual IIdentity GetIdentity()
    {
        var identity = TryGetIdentity();

        if (identity == null)
        {
            throw new AppException("Could not get the 'IIdentity' object from 'HttpContext.Items'.", ExceptionCode.Internal);
        }

        return identity;
    }

    /// <summary>
    /// Asynchronously authorizes a user based on the provided resource policy.
    /// </summary>
    /// <param name="resourcePolicy">The policy against which the user's identity is verified.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <exception cref="AppException">Thrown when the user is not authenticated or does not have the necessary authorization.</exception>
    protected virtual async Task AuthorizeAsync(IResourcePolicy? resourcePolicy)
    {
        // If no resource policy is provided, no authorization is needed.
        if (resourcePolicy == null)
        {
            return;
        }

        var identity = TryGetIdentity();
        var isAuthorized = await resourcePolicy.AuthorizeAsync(identity);

        if (!isAuthorized)
        {
            throw new AppException("The authenticated user lacks the required permissions to execute this operation. Ensure you have the right privileges.", ExceptionCode.Unauthorized);
        }
    }

    /// <summary>
    /// Retrieves the Authorization header from the current HTTP context.
    /// </summary>
    /// <returns>The value of the Authorization header if it exists; otherwise, null.</returns>
    protected string? GetAuthorizationHeader()
    {
        return HttpContext.GetAuthorizationHeader();
    }

    /// <summary>
    /// Retrieves the Bearer token from the Authorization header in the current HTTP context.
    /// </summary>
    /// <returns>The Bearer token if it exists; otherwise, null.</returns>
    protected string? GetBearerToken()
    {
        return HttpContext.GetBearerToken();
    }

}