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
    /// Attempts to retrieve the user's identity from the current HTTP context.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>The user's identity or null if not found.</returns>
    public static IIdentity? TryGetIdentity(this HttpContext context)
    {
        if (context.Items.TryGetValue(WebController.HttpContextIdentityKey, out object? value))
        {
            var identity = value?.TryTypeCast<IIdentity>();

            if (identity != null)
            {
                return identity;
            }
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
    /// <returns>The user's identity.</returns>
    /// <exception cref="AppException">Thrown if the identity is not found.</exception>
    public static IIdentity GetIdentity(this HttpContext context)
    {
        var identity = TryGetIdentity(context);

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
    public static void WriteNoContentResponse(this HttpContext context)
    {
        context.Response.Clear();
        context.Response.StatusCode = 204;
    }

    /// <summary>
    /// Sends an HTTP response with the specified content type and data.
    /// </summary>
    public static async Task WriteTextResponseAsync(this HttpContext context, int statusCode, string contentType, string data)
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
    public static Task WriteJsonResponseAsync(this HttpContext context, int statusCode, string data)
    {
        return WriteTextResponseAsync(context, statusCode, "application/json", data);
    }

    /// <summary>
    /// Sends an HTML response with the specified status code and data.
    /// </summary>
    public static Task WriteHtmlResponseAsync(this HttpContext context, int statusCode, string html)
    {
        return WriteTextResponseAsync(context, statusCode, "text/html", html);
    }

    /// <summary>
    /// Writes an operation result response to the HTTP context. <br/>
    /// This method serializes the provided <see cref="OperationResult"/> and sends it as a JSON response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="operationResult">The operation result to be serialized and written in the response.</param>
    /// <param name="statusCode">The HTTP status code to set for the response.</param>
    /// <returns>A task representing the asynchronous operation of writing the operation result response.</returns>
    /// <remarks>
    /// If <c>AspnetSettings.ExposeNonPublicErrors</c> is false, only errors flagged as "public" are included in the response. <br/>
    /// Additionally, any errors flagged with "debug" are logged using <see cref="ErrorLogger"/>.
    /// </remarks>
    public static Task WriteOperationResponseAsync(this HttpContext context, OperationResult operationResult, int statusCode)
    {
        var debugErrors = operationResult.Errors
            .Where(x => x.ContainsFlags(ErrorFlags.Debug));

        foreach (var item in debugErrors)
        {
            ErrorLogger.Log(item);
        }

        if (!AspnetSettings.ExposeNonPublicErrors)
        {
            operationResult.RemoveErrorsWithoutFlags(ErrorFlags.Public);
        }

        var json = JsonSerializerSingleton.Serialize(operationResult);

        return WriteJsonResponseAsync(context, statusCode, json);
    }

    /// <summary>
    /// Writes a response for a failed operation to the HTTP context. <br/>
    /// This method uses <c>AspnetSettings.FailedOperationStatusCode</c> as the HTTP status code for the response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="result">The operation result representing the failed operation.</param>
    /// <returns>A task representing the asynchronous operation of writing the failed operation response.</returns>
    public static Task WriteFailedOperationResponseAsync(this HttpContext context, OperationResult result)
    {
        return WriteOperationResponseAsync(context, result, AspnetSettings.FailedOperationStatusCode);
    }

    /// <summary>
    /// Writes an exception response to the HTTP context based on the provided exception and application settings. <br/>
    /// This method converts the exception to an <see cref="OperationResult"/> and sends it as a JSON response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="exception">The exception to be processed and potentially written in the response.</param>
    /// <param name="enableExceptionLogging">Indicates whether to log the exception using <see cref="ErrorLogger"/>.</param>
    /// <returns>A task representing the asynchronous operation of writing the exception response.</returns>
    /// <remarks>
    /// If <c>enableExceptionLogging</c> is true, the exception error is flagged as a "debug". <br/>
    /// The HTTP status code for the response is set to 500 (Internal Server Error).
    /// </remarks>
    public static Task WriteExceptionResponseAsync(
        this HttpContext context, 
        Exception exception, 
        bool enableExceptionLogging)
    {
        OperationResult? operationResult = null;

        if (exception is ErrorException errorException)
        {
            operationResult = new OperationResult(errorException.Errors);
        }

        if (operationResult == null)
        {
            var error = new Error(exception);

            operationResult = new OperationResult(error);
        }

        foreach (var error in operationResult.Errors)
        {
            if (enableExceptionLogging)
            {
                error.AddFlags(ErrorFlags.Debug);
            }
            if (AspnetSettings.ExposeExceptions)
            {
                error.AddFlags(ErrorFlags.Public);
            }
        }

        return WriteOperationResponseAsync(context, operationResult, 500);
    }

}
