using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ModularSystem.Core;
using ModularSystem.Core.Logging;
using ModularSystem.Core.AccessManagement;
using System.Text;
using ModularSystem.Web.AccessManagement;

namespace ModularSystem.Web;

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
            ErrorLogger.Log(e);
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
        return HttpContext.TryGetIdentity();
    }

    /// <summary>
    /// Retrieves the identity object from the HttpContext.Items dictionary. Throws an exception if not found.
    /// </summary>
    /// <returns>An IIdentity object.</returns>
    /// <exception cref="AppException">Thrown when the identity object cannot be found in the HttpContext.Items dictionary.</exception>
    protected virtual IIdentity GetIdentity()
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
    /// Read the HTTP body as a JSON string and attempts to deserialized it as a <typeparamref name="T"/> instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected async Task<T?> DeserializeBodyAsJsonAsync<T>() 
    {
        var str = await ReadBodyAsStringAsync();

        if(str == null)
        {
            return default;
        }

        return JsonSerializerSingleton.Deserialize<T>(str);
    }

    /// <summary>
    /// Tries to retrieve a service of type <typeparamref name="T"/> from the ASP.NET Core dependency injection container.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <returns>
    /// The instance of the service if found; otherwise, returns <c>default</c> (which will be <c>null</c> for reference types).
    /// </returns>
    /// <remarks>
    /// This method attempts to resolve a service from the current HTTP context's request services. It's a safe way to
    /// attempt service resolution without throwing exceptions if the service is not registered. Use this method when it's
    /// acceptable for the operation to continue without the service.
    /// </remarks>
    protected T? TryGetService<T>()
    {
        var service = HttpContext.RequestServices.GetService(typeof(T));

        if (service is T tService)
        {
            return tService;
        }

        return default;
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
        var service = TryGetService<T>();

        if (service == null)
        {
            throw new InvalidOperationException($"The required service of type {typeof(T).FullName} could not be resolved.");
        }

        return service;
    }

    //*
    // HTTP response.
    //*

    /// <summary>
    /// Generates an IActionResult representing the outcome of an operation, including any errors, and sends it as a JSON response. <br/>
    /// This method processes the provided <see cref="OperationResult"/> and formats it into a JSON response.
    /// </summary>
    /// <param name="operationResult">The operation result that contains the status and potential errors of the operation.</param>
    /// <param name="statusCode">The HTTP status code to be set for the response.</param>
    /// <returns>An IActionResult that can be returned from an action method.</returns>
    /// <remarks>
    /// If <c>AspnetSettings.ExposeNonPublicErrors</c> is false, only errors flagged as "public" are included in the response. <br/>
    /// Errors flagged as "debug" are logged using <see cref="ErrorLogger"/>.
    /// </remarks>
    protected IActionResult OperationResponse(OperationResult operationResult, int statusCode)
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
        if (!AspnetSettings.ExposeExceptions)
        {
            operationResult.RemoveErrorsWithFlags(ErrorFlags.Exception);
        }

        var json = JsonSerializerSingleton.Serialize(operationResult);

        HttpContext.Response.ContentType = "application/json";
        HttpContext.Response.StatusCode = statusCode;

        return Content(json, "application/json");
    }

    /// <summary>
    /// Generates an IActionResult for a failed operation result and sends it as a JSON response. <br/>
    /// This method uses the <c>AspnetSettings.FailedOperationStatusCode</c> as the HTTP status code for the response.
    /// </summary>
    /// <param name="result">The operation result representing the failed operation.</param>
    /// <returns>An IActionResult that can be returned from an action method.</returns>
    /// <remarks>
    /// This method is typically used when the operation represented by the OperationResult has failed.
    /// </remarks>
    protected IActionResult FailedOperationResponse(OperationResult result)
    {
        return OperationResponse(result, AspnetSettings.FailedOperationStatusCode);
    }

    protected IActionResult FailedOperationResponse(params Error[] errors)
    {
        return FailedOperationResponse(new OperationResult(errors));
    }

    protected IActionResult ErrorResponse(params Error[] errors)
    {
        foreach (var error in errors)
        {
            error.AddFlags(ErrorFlags.Public);
        }

        return FailedOperationResponse(errors);
    }

    /// <summary>
    /// Generates an IActionResult for an exception and sends it as a JSON response. <br/>
    /// The response includes the exception details, formatted as an OperationResult. 
    /// </summary>
    /// <param name="exception">The exception to be processed and included in the response.</param>
    /// <returns>An IActionResult that can be returned from an action method.</returns>
    /// <remarks>
    /// If <c>EnableExceptionLogging</c> is true, the exception error is flagged as a "debug". <br/>
    /// Additionally, if <c>AspnetSettings.ExposeExceptions</c> is true, the error is flagged as "public", making its details visible in the response. <br/>
    /// The HTTP status code for the response is set to 500 (Internal Server Error).
    /// </remarks>
    protected virtual IActionResult ExceptionResponse(Exception exception)
    {
        OnException(exception);

        OperationResult? operationResult = null;

        if (exception is ErrorException errorException)
        {
            operationResult = new OperationResult(errorException.Errors);
        }

        if(operationResult == null)
        {
            var error = new Error(exception);

            operationResult = new OperationResult(error);
        }

        foreach (var error in operationResult.Errors)
        {
            if (EnableExceptionLogging)
            {
                error.AddFlags(ErrorFlags.Debug);
            }

            error.AddFlags(ErrorFlags.Exception);
        }

        return OperationResponse(operationResult, 500);
    }

    /// <summary>
    /// Generates an <see cref="IActionResult"/> containing the provided data transfer object (DTO).
    /// </summary>
    /// <typeparam name="T">The type of the data transfer object.</typeparam>
    /// <param name="value">The data transfer object to include in the response.</param>
    /// <returns>An <see cref="IActionResult"/> that wraps the provided DTO in a standardized format for API responses.</returns>
    protected virtual IActionResult DtoResponse<T>(T? value)
    {
        return Ok(new Dto<T>(value));
    }


}