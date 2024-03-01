using ModularSystem.Core;
using System.Text;
using System.Text.Json;

namespace ModularSystem.Web.Client;

//*
// Base class for defining a HTTP endpoint integration with an endpoint exposed by a subclass of WebController of this library.
//*

/// <summary>
/// Provides a base implementation for RESTful endpoints with a specific request URI and input/output types. <br/>
/// This abstract class extends the functionality of <see cref="RestfulEndpoint{TIn, TOut}"/> by adding a predefined request URI.
/// </summary>
/// <typeparam name="TIn">The type of input data for the endpoint.</typeparam>
/// <typeparam name="TOut">The type of output data produced by the endpoint.</typeparam>
public abstract class EndpointBase<TIn, TOut> : RestfulEndpoint<TIn, TOut>
{
    /// <summary>
    /// Gets the request URI for the current endpoint.
    /// </summary>
    protected URI RequestUri { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointBase{TIn, TOut}"/> class with a specific base URI.
    /// </summary>
    /// <param name="uri">The base URI for the endpoint.</param>
    protected EndpointBase(URI uri)
    {
        RequestUri = uri;
    }

    /// <summary>
    /// Handles the failure response received from an HTTP endpoint asynchronously. <br/>
    /// It attempts to deserialize the response into an <see cref="OperationResult{TOut}"/>. If deserialization fails, a default error is created.
    /// </summary>
    /// <param name="response">The <see cref="HttpResponse"/> object representing the HTTP response from the endpoint.</param>
    /// <returns>A task that represents the asynchronous operation, resulting in an <see cref="OperationResult{TOut}"/>.</returns>
    /// <remarks>
    /// This method should be overridden in derived classes for the following reasons:
    /// <list type="bullet">
    /// <item>
    /// <description>When the operation can yield data even in failure scenarios. The default implementation assumes no data is available when the operation fails.</description>
    /// </item>
    /// <item>
    /// <description>When handling API-specific error structures. Different applications may have unique error response formats that need custom deserialization logic.</description>
    /// </item>
    /// </list>
    /// </remarks>
    protected override async Task<OperationResult<TOut>> ProccessFailureResponseAsync(HttpResponse response)
    {
        var json = await response.ReadAsStringAsync(Encoding.UTF8);

        var options = new JsonSerializerOptions()
        {
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        var operationResult = JsonSerializerSingleton
            .Deserialize<OperationResult<TOut>>(json, options);

        if (operationResult == null)
        {
            var message = $"Failed to process the HTTP API request. Received status code: {response.StatusCode}. Refer to the associated data for detailed information.";
            var responseSummary = await response.CreateSummaryCopyAsync();

            var error = new Error(message)
                .AddJsonData("HTTP Response", responseSummary);

            operationResult = new(error);
        }

        return operationResult;
    }

}

//*
// Helpers for defining API endpoints.
//*

/// <summary>
/// Represents the base endpoint for API operations that neither require an input nor produce an output.
/// This class simplifies the creation of endpoints that execute side-effecting operations without expecting any input or returning any results.
/// </summary>
/// <remarks>
/// Derive from this class when creating endpoints that perform actions where neither input data nor an output data structure are expected.
/// </remarks>
public abstract class EndpointBase : EndpointBase<Core.Void, Core.Void>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointBase"/> class with the specified URI.
    /// </summary>
    /// <param name="uri">The URI associated with this endpoint.</param>
    protected EndpointBase(URI uri) : base(uri)
    {
        // Intentionally left blank.
    }

    /// <inheritdoc/>
    public Task RunAsync()
    {
        return RunAsync(new Core.Void());
    }

    /// <inheritdoc/>
    protected abstract HttpRequest CreateRequest();

    /// <inheritdoc/>
    protected override HttpRequest CreateRequest(Core.Void input)
    {
        return CreateRequest(input);
    }

    /// <inheritdoc/>
    protected override Task<Core.Void> DeserializeResponseAsync(HttpResponse response)
    {
        return Task.FromResult(new Core.Void());
    }

}
