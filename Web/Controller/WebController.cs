using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using ModularSystem.Core;
using ModularSystem.Web.AccessManagement;
using ModularSystem.Web.AccessManagement.Attributes;
using ModularSystem.Web.AccessManagement.Extensions;
using ModularSystem.Web.Attributes;
using ModularSystem.Web.Responses;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace ModularSystem.Web.Controllers;

/// <summary>
/// Represents a base web controller that provides utility methods and exception handling mechanisms.
/// </summary>
public abstract class WebController : ControllerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebController"/> class.
    /// </summary>
    protected WebController()
    {

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
    /// Retrieves the identity object from the HttpContext.Items dictionary. Throws an exception if not found.
    /// </summary>
    /// <returns>An IIdentity object.</returns>
    /// <exception cref="AppException">Thrown when the identity object cannot be found in the HttpContext.Items dictionary.</exception>
    protected virtual IIdentity? GetIdentity()
    {
        return HttpContext.GetIdentity();
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

    /// <summary>
    /// Reads the request body as a string asynchronously, using the specified encoding.
    /// </summary>
    /// <remarks>
    /// This method asynchronously reads the request body and returns the content as a string.
    /// If the body is empty or contains only whitespace, this method returns null.
    /// It uses UTF8 encoding by default, which is suitable for a wide range of text data, but a different encoding can be specified if necessary.
    /// This method is particularly useful for processing textual data from request bodies, such as JSON or XML.
    /// </remarks>
    /// <param name="encoding">The character encoding to use when reading the request body. Defaults to UTF8 if not provided.</param>
    /// <returns>
    /// A task that represents the asynchronous read operation. The task result contains the request body as a string, or null if the body is empty or contains only whitespace.
    /// </returns>
    protected async Task<string?> ReadBodyAsStringAsync(Encoding? encoding = null)
    {
        using var reader = new StreamReader(Request.Body, encoding ?? Encoding.UTF8);

        var text = await reader.ReadToEndAsync();

        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        return text;
    }

    /// <summary>
    /// Deserializes the request body as JSON into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize the JSON into.</typeparam>
    /// <param name="options">Optional <see cref="JsonSerializerOptions"/> to use for deserialization. If <c>null</c>, default options are used.</param>
    /// <returns>
    /// The deserialized object of type <typeparamref name="T"/> if the body is valid JSON; otherwise, <c>null</c>.
    /// </returns>
    protected async Task<T?> DeserializeBodyAsJsonAsync<T>(JsonSerializerOptions? options = null)
    {
        var str = await ReadBodyAsStringAsync();

        if (str == null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(str, options);
    }

    /// <summary>
    /// Retrieves a service of type <typeparamref name="TService"/> from the request's service provider.
    /// </summary>
    /// <typeparam name="TService">The type of the service to retrieve.</typeparam>
    /// <returns>
    /// An instance of the service if it is registered; otherwise, <c>null</c>.
    /// </returns>
    protected TService? GetService<TService>()
    {
        return HttpContext.RequestServices.GetService<TService>();
    }

    /// <summary>
    /// Attempts to retrieve a service of type <typeparamref name="T"/> from the request's service provider.
    /// </summary>
    /// <typeparam name="T">The type of the service to retrieve.</typeparam>
    /// <param name="service">When this method returns, contains the service instance if found; otherwise, <c>null</c>.</param>
    /// <returns>
    /// <c>true</c> if the service was successfully retrieved; otherwise, <c>false</c>.
    /// </returns>
    protected bool TryGetService<T>([MaybeNullWhen(false)] out T service)
    {
        service = HttpContext.RequestServices.GetService<T>();
        return service != null;
    }

    /// <summary>
    /// Retrieves a required service of type <typeparamref name="T"/> from the request's service provider.
    /// </summary>
    /// <typeparam name="T">The type of the service to retrieve.</typeparam>
    /// <returns>
    /// The service instance if it is registered; otherwise, throws an <see cref="InvalidOperationException"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the service of type <typeparamref name="T"/> cannot be resolved from the service provider.
    /// </exception>
    protected T GetRequiredService<T>()
    {
        if (!TryGetService<T>(out var service))
        {
            throw new InvalidOperationException($"Could not resolve service of type '{typeof(T).Name}' from the service provider.");
        }

        return service;
    }

    //*
    // HTTP responses.
    //*

    /*
     * Problem details response.
     */

    protected IActionResult ProblemResponse(
        int statusCode,
        ProblemResponse response,
        JsonSerializerOptions? options = null)
    {
        HttpContext.Response.StatusCode = statusCode;
        return Content(
            content: JsonSerializer.Serialize(response, options),
            contentType: "application/json");
    }

    protected IActionResult ProblemResponse(
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

        return ProblemResponse(statusCode, response, options);
    }

    protected IActionResult ProblemResponse(
        int statusCode,
        Error error,
        JsonSerializerOptions? options = null)
    {
        return ProblemResponse(
            statusCode: statusCode,
            response: Responses.ProblemResponse.FromError(error),
            options: options);
    }

    protected IActionResult ProblemResponse(
        int statusCode,
        IEnumerable<Error> errors,
        JsonSerializerOptions? options = null)
    {
        return ProblemResponse(
            statusCode: statusCode,
            response: Responses.ProblemResponse.FromErrors(errors),
            options: options);
    }

    protected IActionResult ProblemResponse(
        int statusCode,
        Exception exception,
        JsonSerializerOptions? options = null)
    {
        return ProblemResponse(
            statusCode: statusCode,
            response: Responses.ProblemResponse.FromException(exception),
            options: options);
    }

    protected IActionResult ProblemResponse(
        int statusCode,
        string message,
        JsonSerializerOptions? options = null)
    {
        return ProblemResponse(
            statusCode: statusCode,
            response: Responses.ProblemResponse.FromError(new Error(title: message)),
            options: options);
    }

}