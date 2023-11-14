using ModularSystem.Core;
using ModularSystem.Web.Http;

namespace ModularSystem.Web;

/// <summary>
/// Defines a RESTful endpoint that processes input of type <typeparamref name="TIn"/> and produces output of type <typeparamref name="TOut"/>.
/// </summary>
/// <typeparam name="TIn">The input type for the endpoint.</typeparam>
/// <typeparam name="TOut">The output type of the endpoint.</typeparam>
public interface IRestfulEndpoint<TIn, TOut>
{
    /// <summary>
    /// Executes the RESTful operation asynchronously.
    /// </summary>
    /// <param name="input">The input data for the operation.</param>
    /// <returns>A task that represents the asynchronous operation and produces output of type <typeparamref name="TOut"/>.</returns>
    Task<TOut> RunAsync(TIn input);
}

/// <summary>
/// Represents the configuration settings for a RESTful endpoint.
/// </summary>
public class EndpointConfiguration
{
    /// <summary>
    /// Gets or sets the request URI for the endpoint.
    /// </summary>
    public Http.Uri RequestUri { get; set; }

    /// <summary>
    /// Gets or sets the header configuration for the endpoint.
    /// </summary>
    public HttpHeader? Header { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointConfiguration"/> class with the specified base URI.
    /// </summary>
    /// <param name="baseUri">The base URI for the endpoint.</param>
    public EndpointConfiguration(Http.Uri baseUri)
    {
        RequestUri = baseUri.Copy();
        Header = null;
    }

    /// <summary>
    /// Creates a copy of the current endpoint configuration.
    /// </summary>
    /// <returns>A new instance of <see cref="EndpointConfiguration"/> that is a copy of the current instance.</returns>
    public EndpointConfiguration Copy()
    {
        return new EndpointConfiguration(RequestUri)
        {
            Header = Header?.Copy(),
        };
    }
}

/// <summary>
/// Provides a base implementation of the <see cref="IRestfulEndpoint{TIn, TOut}"/> interface.
/// This class streamlines the process of performing HTTP API requests by handling the boilerplate logic,
/// including request creation, response handling, and error handling. Additionally, it offers hooks
/// for various stages of the request-response cycle, allowing for custom behaviors and modifications
/// at key points in the process.
/// </summary>
/// <remarks>
/// Implementers can leverage the provided hooks to introduce custom logic before and after key operations,
/// such as request creation, response deserialization, and error handling.
/// </remarks>
/// <typeparam name="TIn">The input type for the endpoint.</typeparam>
/// <typeparam name="TOut">The output type of the endpoint.</typeparam>
public abstract class RestfulEndpoint<TIn, TOut> : IRestfulEndpoint<TIn, TOut>
{
    /// <summary>
    /// Disposes any resources used by this instance.
    /// </summary>
    public void Dispose()
    {

    }

    /// <inheritdoc />
    /// <summary>
    /// Asynchronously executes the API request with the given input and returns the response.
    /// </summary>
    /// <param name="input">The input parameters for the API request.</param>
    /// <returns>A <see cref="Task{TOut}"/> representing the asynchronous operation.</returns>
    /// <exception cref="BoxedException">Thrown when the HTTP response indicates a failure.</exception>
    /// <exception cref="Exception">Thrown for any other exception scenario.</exception>
    public virtual async Task<TOut> RunAsync(TIn input)
    {
        HttpRequest? request = null;
        HttpResponse? response = null;

        try
        {
            var client = GetRequester();

            request = CreateRequest(input);

            OnRequestCreated(request);
            await OnRequestCreatedAsync(request);

            response = await client.SendAsync(request);

            OnResponse(response);
            await OnResponseAsync(response);

            if (!response.IsSuccess)
            {
                throw new BoxedException(await HandleFailureResponseAsync(response));
            }

            BeforeDeserialize(response);
            await BeforeDeserializeAsync(response);

            var payload = await DeserializeResponseAsync(response);

            AfterDeserialize(response, payload);
            await AfterDeserializeAsync(response, payload);

            return payload;
        }
        catch (Exception e)
        {
            if (e is BoxedException)
            {
                throw ((BoxedException)e).Exception;
            }

            throw await HandleExceptionAsync(e, request, response);
        }
        finally
        {
            response?.Dispose();
        }
    }

    /// <summary>
    /// Handles the failure response received from an HTTP endpoint.
    /// </summary>
    /// <param name="response">The <see cref="HttpResponse"/> object representing the HTTP response from the endpoint.</param>
    /// <returns>An exception that represents the error occurred during the handling of the HTTP response.</returns>
    /// <remarks>
    /// Implementations of this method should inspect the provided HTTP response, and return an appropriate exception
    /// that captures the essence of the error or issue. <br/>
    /// This exception will typically be propagated up to the calling code for further handling or logging.
    /// </remarks>
    protected abstract Task<Exception> HandleFailureResponseAsync(HttpResponse response);

    /// <summary>
    /// Retrieves an instance of the HttpRequester class to send HTTP requests.
    /// </summary>
    /// <returns>An instance of <see cref="HttpRequester"/>.</returns>
    protected virtual HttpRequester GetRequester()
    {
        return HttpRequester.Singleton;
    }

    /// <summary>
    /// Creates the HttpRequest object from the provided input.
    /// </summary>
    /// <param name="input">The input parameters for creating the request.</param>
    /// <returns>An HttpRequest object.</returns>
    protected abstract HttpRequest CreateRequest(TIn input);

    /// <summary>
    /// Invoked after the HttpRequest object is constructed and before it's sent.
    /// </summary>
    /// <param name="request">The constructed HttpRequest object.</param>
    protected virtual void OnRequestCreated(HttpRequest request)
    {
        return;
    }

    /// <summary>
    /// Asynchronously invoked after the HttpRequest object is constructed and before it's sent.
    /// </summary>
    /// <param name="request">The constructed HttpRequest object.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnRequestCreatedAsync(HttpRequest request)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked immediately after receiving the HttpResponse, regardless of its status code.
    /// </summary>
    /// <param name="response">The received HttpResponse.</param>
    protected virtual void OnResponse(HttpResponse response)
    {
        return;
    }

    /// <summary>
    /// Asynchronously invoked after receiving the HttpResponse, regardless of its status code.
    /// </summary>
    /// <param name="response">The received HttpResponse.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnResponseAsync(HttpResponse response)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked before the deserialization process of a successful response. <br/>
    /// This method provides an opportunity to perform any pre-processing or validation on the raw response data.
    /// </summary>
    /// <param name="response">The received HttpResponse that will be deserialized.</param>
    protected virtual void BeforeDeserialize(HttpResponse response)
    {
        return;
    }

    /// <summary>
    /// Asynchronously invoked before the deserialization process of a successful response. <br/>
    /// This method provides an opportunity to perform any asynchronous pre-processing or validation on the raw response data.
    /// </summary>
    /// <param name="response">The received HttpResponse that will be deserialized.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task BeforeDeserializeAsync(HttpResponse response)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked after the deserialization process of a successful response.
    /// </summary>
    /// <remarks>
    /// This method provides a synchronization point for performing any post-processing or validation on the deserialized payload.
    /// It is designed to be a counterpart to the asynchronous <see cref="AfterDeserializeAsync"/> method, offering a synchronous alternative.
    /// Implementers can use this method to add custom logic that operates on the deserialized data, such as additional validation, 
    /// transformation, or augmentation, in scenarios where asynchronous processing is not required or desired.
    /// </remarks>
    /// <param name="response">The HttpResponse from which the payload was deserialized.</param>
    /// <param name="payload">The deserialized payload of type <typeparamref name="TOut"/>.</param>
    protected virtual void AfterDeserialize(HttpResponse response, TOut payload)
    {
        return;
    }

    /// <summary>
    /// Asynchronously invoked after the deserialization process of a successful response.
    /// </summary>
    /// <remarks>
    /// This method serves as a hook for performing any post-processing or validation on the deserialized payload.
    /// It allows implementers to add custom logic that executes after the API response has been successfully deserialized
    /// but before the control is returned to the caller. This is particularly useful for scenarios where the deserialized
    /// data needs to be augmented, validated, or transformed before being used.
    /// </remarks>
    /// <param name="response">The HttpResponse from which the payload was deserialized.</param>
    /// <param name="payload">The deserialized payload of type <typeparamref name="TOut"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task AfterDeserializeAsync(HttpResponse response, TOut payload)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously deserializes the successful API response.
    /// </summary>
    /// <remarks>
    /// This method provides an asynchronous implementation for deserializing the API response and is directly 
    /// invoked within the request pipeline by <see cref="RunAsync"/>.
    /// If not overridden in a subclass, this method defaults to invoking the synchronous 
    /// <see cref="DeserializeResponse"/> method.
    /// </remarks>
    /// <param name="response">The HttpResponse representing the successful API response.</param>
    /// <returns>A task representing the asynchronous operation with a result of type <typeparamref name="TOut"/> containing the deserialized response.</returns>
    protected virtual Task<TOut> DeserializeResponseAsync(HttpResponse response)
    {
        return response.DeserializeAsJsonAsync<TOut>();
    }

    /// <summary>
    /// Synchronously deserializes the successful API response.
    /// </summary>
    /// <remarks>
    /// This method provides a synchronous implementation for deserializing the response. It is intended to be invoked 
    /// internally by the default <see cref="DeserializeResponseAsync"/> implementation. If you override 
    /// <see cref="DeserializeResponseAsync"/>, this method might never be executed. Ensure you provide the desired 
    /// behavior in your asynchronous override if you choose to do so.
    /// </remarks>
    /// <param name="response">The HttpResponse representing the successful API response.</param>
    /// <returns>The deserialized response of type <see cref="TOut"/>.</returns>
    /// <exception cref="NotImplementedException">Thrown when this method has not been overridden in a subclass and gets invoked.</exception>
    protected virtual TOut DeserializeResponse(HttpResponse response)
    {
        throw new NotImplementedException("The deserialize method was not implemented.");
    }

    /// <summary>
    /// Handles exceptions that occur during the API request and response cycle.
    /// </summary>
    /// <param name="e">The original exception.</param>
    /// <param name="request">The HttpRequest object, if available.</param>
    /// <param name="response">The HttpResponse object, if available.</param>
    /// <returns>An exception that encapsulates additional information and context.</returns>
    protected virtual async Task<Exception> HandleExceptionAsync(Exception e, HttpRequest? request, HttpResponse? response)
    {
        var error = $"Failed to process the HTTP API request. Refer to the associated metadata and inner exception for detailed information.";
        var exception = new AppException(error, ExceptionCode.Internal, e);

        if (response != null)
        {
            exception.AddData(await response.GetSummaryAsync());
        }
        else if (request != null)
        {
            exception.AddData(request);
        }

        return exception;
    }

    /// <summary>
    /// Internal class to wrap exceptions.
    /// </summary>
    internal class BoxedException : Exception
    {
        /// <summary>
        /// Gets or sets the wrapped exception.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxedException"/> class.
        /// </summary>
        /// <param name="exception">The exception to wrap.</param>
        public BoxedException(Exception exception)
        {
            Exception = exception;
        }
    }
}