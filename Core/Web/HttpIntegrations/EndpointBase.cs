using ModularSystem.Core;
using System.Text;
using System.Text.Json;

namespace ModularSystem.Web.Client;

//*
// Base class for defining a HTTP endpoint integration with an endpoint exposed by a subclass of WebController of this library.
//*

/// <summary>
/// Represents a base endpoint configuration for interacting with RESTful services.
/// This base class provides built-in functionality for deserialization, error handling, and response processing.
/// </summary>
/// <typeparam name="TIn">The input type for API requests.</typeparam>
/// <typeparam name="TOut">The output type expected from API responses.</typeparam>
public abstract class EndpointBase<TIn, TOut> : RestfulEndpoint<TIn, TOut>
{
    /// <summary>
    /// Gets the request URI for the current endpoint.
    /// </summary>
    protected Http.Uri RequestUri { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointBase{TIn, TOut}"/> class.
    /// </summary>
    /// <param name="uri">The base URI for the endpoint.</param>
    protected EndpointBase(Http.Uri uri)
    {
        RequestUri = uri;
    }

    /// <summary>
    /// Handles failure responses by attempting to deserialize the returned error and convert it into an AppException.
    /// </summary>
    /// <param name="response">The failed HTTP response.</param>
    /// <returns>An exception that represents the error.</returns>
    protected override Exception HandleFailureResponse(HttpResponse response)
    {
        try
        {
            var json = response.ReadAsString(Encoding.UTF8);
            var deserializedException = JsonSerializer.Deserialize<SerializableAppException>(json, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

            if (deserializedException == null || deserializedException.Message.IsEmpty())
            {
                return new AppException($"Failed to process the HTTP API request. Received status code: {response.StatusCode}. Refer to the associated metadata and inner exception for detailed information.", ExceptionCode.Internal)
                    .AddData(response.GetSummary());
            }

            return deserializedException.ToAppException();
        }
        catch (Exception e)
        {
            return new AppException($"Failed to process the HTTP API request. Received status code: {response.StatusCode}. Refer to the associated metadata and inner exception for detailed information.",
                ExceptionCode.Internal, e).AddData(response.GetSummary());
        }
    }

    /// <summary>
    /// Deserializes the response from the HTTP request asynchronously.
    /// </summary>
    /// <param name="response">The HTTP response to deserialize.</param>
    /// <returns>A task that represents the asynchronous deserialization operation. The value of TResult contains the deserialized response.</returns>
    protected override async Task<TOut> DeserializeResponseAsync(HttpResponse response)
    {
        return (await response.DeserializeAsJsonAsync(typeof(TOut), Encoding.UTF8)).TypeCast<TOut>();
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
    protected EndpointBase(Http.Uri uri) : base(uri)
    {
        // Intentionally left blank.
    }

    /// <summary>
    /// Executes the endpoint operation asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task RunAsync()
    {
        return RunAsync(new Core.Void());
    }

    /// <summary>
    /// When overridden in a derived class, creates an <see cref="HttpRequest"/> object 
    /// representing the API request associated with this endpoint.
    /// </summary>
    /// <returns>The created <see cref="HttpRequest"/> object.</returns>
    protected abstract HttpRequest CreateRequest();

    /// <summary>
    /// Creates an <see cref="HttpRequest"/> using the provided input.
    /// </summary>
    /// <param name="input">The input of type <see cref="Core.Void"/>.</param>
    /// <returns>The created <see cref="HttpRequest"/> object.</returns>
    protected override HttpRequest CreateRequest(Core.Void input)
    {
        return CreateRequest(input);
    }

    /// <summary>
    /// Deserializes the response to an instance of <see cref="Core.Void"/>, effectively capturing no data.
    /// </summary>
    /// <param name="response">The received <see cref="HttpResponse"/>.</param>
    /// <returns>An instance of <see cref="Core.Void"/>.</returns>
    protected override Core.Void DeserializeResponse(HttpResponse response)
    {
        return new Core.Void();
    }
}

/// <summary>
/// Represents a base endpoint configuration specialized for APIs that only take input and do not return data.
/// This class streamlines the deserialization process by assuming the output is always <see cref="Core.Void"/>.
/// </summary>
/// <typeparam name="TIn">The input type for API requests.</typeparam>
public abstract class EndpointBaseIn<TIn> : EndpointBase<TIn, Core.Void>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointBaseIn{TIn}"/> class.
    /// </summary>
    /// <param name="uri">The base URI for the endpoint.</param>
    protected EndpointBaseIn(Http.Uri uri) : base(uri)
    {
    }

    /// <summary>
    /// Deserializes the API response. Since the expected output type is <see cref="Core.Void"/>, 
    /// this method always returns a new instance of <see cref="Core.Void"/>.
    /// </summary>
    /// <param name="response">The HTTP response to deserialize.</param>
    /// <returns>An instance of <see cref="Core.Void"/>.</returns>
    protected override Core.Void DeserializeResponse(HttpResponse response)
    {
        return new Core.Void();
    }
}

/// <summary>
/// Represents a base endpoint configuration specialized for APIs that only return data (no input).
/// This class streamlines the execution process by assuming the input is always <see cref="Core.Void"/>.
/// </summary>
/// <typeparam name="TOut">The output type expected from API responses.</typeparam>
public abstract class EndpointBaseOut<TOut> : EndpointBase<Core.Void, TOut>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointBaseOut{TOut}"/> class.
    /// </summary>
    /// <param name="uri">The base URI for the endpoint.</param>
    protected EndpointBaseOut(Http.Uri uri) : base(uri)
    {
    }

    /// <summary>
    /// Executes the API request with the default <see cref="Core.Void"/> input.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The value of TResult contains the API response.</returns>
    public Task<TOut> RunAsync()
    {
        return RunAsync(new Core.Void());
    }

    /// <summary>
    /// Creates an HTTP request for the endpoint.
    /// Derived classes must implement this method to specify the request details.
    /// </summary>
    /// <returns>The created HTTP request.</returns>
    protected abstract HttpRequest CreateRequest();

    /// <summary>
    /// Invokes the overridable <see cref="CreateRequest()"/> method to create an HTTP request with the provided input.
    /// </summary>
    /// <param name="input">The input for the API request. Expected to be <see cref="Core.Void"/>.</param>
    /// <returns>The created HTTP request.</returns>
    protected override HttpRequest CreateRequest(Core.Void input)
    {
        return CreateRequest(input);
    }
}