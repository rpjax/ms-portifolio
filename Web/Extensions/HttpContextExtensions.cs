using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ModularSystem.Core;
using ModularSystem.Core.Extensions;
using ModularSystem.Web.Responses;
using System.Text;
using System.Text.Json;
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

        if (authorizationHeader is null)
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

    /*
     * Problem details response.
     */

    public static Task WriteProblemResponseAsync(
        this HttpContext context,
        int statusCode,
        ProblemResponse response,
        JsonSerializerOptions? options = null)
    {
        return WriteJsonResponseAsync(
            context: context,
            statusCode: statusCode,
            data: JsonSerializer.Serialize(response, options));
    }

    public static Task WriteProblemResponseAsync(
        this HttpContext context,
        int statusCode,
        string? title = null,
        string? type = null,
        string? detail = null,
        IEnumerable<Error>? errors = null,
        JsonSerializerOptions? options = null)
    {
        var response = new ProblemResponse(
            title: title,
            type: type,
            detail: detail,
            errors: errors);

        return WriteProblemResponseAsync(
            context: context,
            statusCode: statusCode,
            response: response,
            options: options);
    }

    public static Task WriteProblemResponseAsync(
        this HttpContext context,
        int statusCode,
        Error error,
        JsonSerializerOptions? options = null)
    {
        return WriteProblemResponseAsync(
            context: context,
            statusCode: statusCode,
            response: ProblemResponse.FromError(error),
            options: options);
    }

    public static Task WriteProblemResponseAsync(
        this HttpContext context,
        int statusCode,
        IEnumerable<Error> errors,
        JsonSerializerOptions? options = null)
    {
        return WriteProblemResponseAsync(
            context: context,
            statusCode: statusCode,
            response: ProblemResponse.FromErrors(errors),
            options: options);
    }

    public static Task WriteProblemResponseAsync(
        this HttpContext context,
        int statusCode,
        Exception exception,
        JsonSerializerOptions? options = null)
    {
        return WriteProblemResponseAsync(
            context: context,
            statusCode: statusCode,
            response: ProblemResponse.FromException(exception),
            options: options);
    }

    public static Task WriteProblemResponseAsync(
        this HttpContext context,
        int statusCode,
        string message,
        JsonSerializerOptions? options = null)
    {
        return WriteProblemResponseAsync(
            context: context,
            statusCode: statusCode,
            response: ProblemResponse.FromError(new Error(title: message)),
            options: options);
    }
}
