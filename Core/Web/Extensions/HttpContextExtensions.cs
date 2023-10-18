using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ModularSystem.Core;
using ModularSystem.Core.Logging;
using ModularSystem.Core.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace ModularSystem.Web;

/// <summary>
/// Provides extension methods for <see cref="HttpContext"/> to facilitate common tasks such as extracting authorization details, cookies, and query parameters.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Regex for identification of bearer authorization headers.
    /// </summary>
    public static Regex BearerRegex = new Regex("(?i)bearer", RegexOptions.Compiled);

    /// <summary>
    /// Retrieves the value of a specified query parameter from the current HTTP request.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="name">Name of the query parameter.</param>
    /// <returns>Value of the specified query parameter or null if not found.</returns>
    public static string? GetQueryParam(this HttpContext context, string name)
    {
        if (context.Request.Query.TryGetValue(name, out StringValues stringValue))
        {
            return stringValue.FirstOrDefault();

        }

        return null;
    }

    /// <summary>
    /// Retrieves the value of a specified cookie from the current HTTP request.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="name">Name of the cookie.</param>
    /// <returns>Value of the specified cookie or null if not found.</returns>
    public static string? GetCookie(this HttpContext context, string name)
    {
        if (context.Request.Cookies.TryGetValue(name, out string? cookie))
        {
            return cookie;
        }

        return null;
    }

    /// <summary>
    /// Retrieves the value of the Authorization header from the current HTTP request.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>Value of the Authorization header or null if not present.</returns>
    public static string? GetAuthorizationHeader(this HttpContext context)
    {
        var authorizationHeaderEnumerable = context.Request.Headers.Where(x => x.Key.ToLower() == "authorization");

        if (authorizationHeaderEnumerable.IsEmpty())
        {
            return null;
        }

        return authorizationHeaderEnumerable.First().Value;
    }

    /// <summary>
    /// Extracts the Bearer token from the Authorization header.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>The Bearer token or null if not present.</returns>
    public static string? GetBearerToken(this HttpContext context)
    {
        var authorizationHeader = context.GetAuthorizationHeader();

        if (authorizationHeader == null)
        {
            return null;
        }

        var shouldRemoveBearerWord = authorizationHeader.ToLower().Trim().StartsWith("bearer");

        if (shouldRemoveBearerWord)
        {
            var match = BearerRegex.Match(authorizationHeader);

            return authorizationHeader.Substring(match.Index + match.Length).Replace(" ", "");
        }

        return authorizationHeader;
    }

    /// <summary>
    /// Attempts to retrieve the user's identity from the current HTTP context. If not found in the context's items, <br/>
    /// it can optionally check the Authorization header for a valid token and resolve the identity from that.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="fromItems">If true, the method will try to read the token from the items contained in the HttpContext.</param>
    /// <param name="fromHeaders">If true, the method will try to read the token from the Authorization header and resolve the identity if not found in the context's items.</param>
    /// <returns>The user's identity or null if not found.</returns>
    public static IIdentity? TryGetIdentity(this HttpContext context, bool fromItems = true, bool fromHeaders = false)
    {
        if (fromItems && context.Items.TryGetValue(WebController.HTTP_CONTEXT_IDENTITY_KEY, out object? value))
        {
            var identity = value?.TryTypeCast<IIdentity>();

            if (identity != null)
            {
                return identity;
            }
        }

        if (!fromHeaders)
        {
            return null;
        }

        if (!DependencyContainer.TryGetInterface<IIamSystem>(out var iam))
        {
            return null;
        }

        var token = iam.AuthenticationProvider.GetToken(context);

        if (token == null)
        {
            return null;
        }

        if (token.IsExpired())
        {
            return null;
        }

        return iam.AuthenticationProvider.GetIdentity(token);
    }

    /// <summary>
    /// Retrieves the user's identity from the current HTTP context. Throws an exception if not found.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="readFromHeaderIfNotFound">If true, the method will try to read the token from the Authorization header and resolve the identity if not found in the context's items.</param>
    /// <returns>The user's identity.</returns>
    /// <exception cref="AppException">Thrown if the identity is not found.</exception>
    public static IIdentity GetIdentity(this HttpContext context, bool readFromHeaderIfNotFound = false)
    {
        var identity = TryGetIdentity(context, readFromHeaderIfNotFound);

        if (identity == null)
        {
            throw new AppException("Could not get the 'IIdentity' object from 'HttpContext.Items'.", ExceptionCode.Internal);
        }

        return identity;
    }

    /// <summary>
    /// Sends an HTTP response with no content.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public static void WriteNoContentResponse(HttpContext context)
    {
        context.Response.Clear();
        context.Response.StatusCode = 204;
    }

    /// <summary>
    /// Sends an HTTP response with the specified content type and data.
    /// </summary>
    public static async Task WriteTextResponseAsync(HttpContext context, int statusCode, string contentType, string data)
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
    public static Task WriteJsonResponseAsync(HttpContext context, int statusCode, string data)
    {
        return WriteTextResponseAsync(context, statusCode, "application/json", data);
    }

    /// <summary>
    /// Sends an HTML response with the specified status code and data.
    /// </summary>
    public static Task WriteHtmlResponseAsync(HttpContext context, int statusCode, string html)
    {
        return WriteTextResponseAsync(context, statusCode, "text/html", html);
    }

    /// <summary>
    /// Writes an error response to the client based on the provided exception.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="exception">The exception to be processed and presented to the client.</param>
    /// <returns>A task representing the asynchronous operation of writing the error response.</returns>
    /// <remarks>
    /// This method first converts the given exception into a standardized application exception using <see cref="Exception.ToAppException"/>.
    /// It then determines the appropriate HTTP status code and formats the exception as a JSON response.
    /// If the exception represents an internal error, it will be logged for further analysis.
    /// </remarks>
    public static Task WriteErrorResponseAsync(this HttpContext context, Exception exception)
    {
        var appException = exception.ToAppException();
        var statusCode = AppExceptionPresenter.GetStatusCodeFrom(appException);
        var json = AppExceptionPresenter.ToJson(appException);

        if (appException.Code == ExceptionCode.Internal)
        {
            ExceptionLogger.Log(appException);
        }

        return WriteJsonResponseAsync(context, statusCode, json);
    }

}
