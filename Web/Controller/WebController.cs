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
    protected virtual IIdentity GetIdentity()
    {
        var identity = HttpContext.GetIdentity();

        if (identity == null)
        {
            throw new InvalidOperationException("Could not get the 'IIdentity' object from 'HttpContext.Items'.");
        }

        return identity;
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
    /// Read the HTTP body as a JSON string and attempts to deserialized it as a <typeparamref name="T"/> instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected async Task<T?> DeserializeBodyAsJsonAsync<T>()
    {
        var str = await ReadBodyAsStringAsync();

        if (str == null)
        {
            return default;
        }

        return JsonSerializerSingleton.Deserialize<T>(str);
    }


    /// <summary>
    /// Tries to retrieve a service of the specified type from the ASP.NET Core dependency injection container.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <param name="service">When this method returns, contains the service instance if the service is found, or null if the service is not found. This parameter is passed uninitialized.</param>
    /// <returns>true if the service is found; otherwise, false.</returns>
    /// <remarks>
    /// This method is useful for optional dependencies that might not be registered in the application's service container. It allows conditional logic based on the availability of a service without throwing exceptions if the service is not registered.
    /// </remarks>
    protected bool TryGetService<T>([MaybeNullWhen(false)] out T service) 
    {
        //new Dictionary<string, string>().TryGetValue
        var value = HttpContext.RequestServices.GetService<T>();

        if(value is null)
        {
            service = default;
            return false;
        }

        service = value;
        return true;
    }

    /// <summary>
    /// Retrieves a service of type <typeparamref name="T"/> from the ASP.NET Core dependency injection container.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <returns>
    /// The instance of the service.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the service of type <typeparamref name="T"/> cannot be resolved from the service provider.
    /// </exception>
    /// <remarks>
    /// This method ensures that a service of the specified type is returned. It uses <see cref="TryGetService{T}"/> 
    /// internally to attempt service resolution and throws an <see cref="InvalidOperationException"/> if the service
    /// cannot be found. Use this method when the requested service is essential for the operation to proceed.
    /// </remarks>
    protected T GetService<T>() 
    {
        if(!TryGetService<T>(out var service))
        {
            throw new InvalidOperationException($"Could not resolve service of type '{typeof(T).Name}' from the service provider.");      
        }

        return service;
    }

    //*
    // HTTP response.
    //*

    protected IActionResult ProblemResponse(int statusCode, ErrorResponse response)
    {
        HttpContext.Response.StatusCode = statusCode;
        return Content(JsonSerializer.Serialize(response), "application/json");
    }

    protected IActionResult ProblemResponse(int statusCode, params Error[] errors)
    {
        return ProblemResponse(statusCode, ErrorResponse.FromErrors(errors));
    }

    protected IActionResult ProblemResponse(int statusCode, Exception exception)
    {
        return ProblemResponse(statusCode, ErrorResponse.FromException(exception));
    }

}