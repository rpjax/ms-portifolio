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
/// Provides a base implementation of the <see cref="IRestfulEndpoint{TIn, TOut}"/> interface. <br/>
/// This class handles the boilerplate logic for performing HTTP API requests,
/// including request creation, response handling, and error handling.
/// </summary>
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

            response = await client.SendAsync(request);
            OnResponse(response);

            if (!response.IsSuccess)
            {
                throw new BoxedException(HandleFailureResponse(response));
            }

            BeforeDeserialize(response);
            return await DeserializeResponseAsync(response);
        }
        catch (Exception e)
        {
            if (e is BoxedException)
            {
                throw ((BoxedException)e).Exception;
            }

            throw HandleException(e, request, response);
        }
        finally
        {
            response?.Dispose();
        }
    }

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
    /// Called after the HttpRequest object is created and before it is sent.
    /// </summary>
    /// <param name="request">The HttpRequest object.</param>
    protected virtual void OnRequestCreated(HttpRequest request)
    {

    }

    /// <summary>
    /// Called right after the response is created regardless of its status code.
    /// </summary>
    /// <param name="response"></param>
    protected virtual void OnResponse(HttpResponse response)
    {

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
    protected abstract Exception HandleFailureResponse(HttpResponse response);

    /// <summary>
    /// Called before the deserialization of a successfull response.
    /// </summary>
    /// <param name="response"></param>
    protected virtual void BeforeDeserialize(HttpResponse response)
    {

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
        return Task.FromResult(DeserializeResponse(response));
    }

    /// <summary>
    /// Handles exceptions that occur during the API request and response cycle.
    /// </summary>
    /// <param name="e">The original exception.</param>
    /// <param name="request">The HttpRequest object, if available.</param>
    /// <param name="response">The HttpResponse object, if available.</param>
    /// <returns>An exception that encapsulates additional information and context.</returns>
    protected virtual Exception HandleException(Exception e, HttpRequest? request, HttpResponse? response)
    {
        var error = $"Failed to process the HTTP API request. Refer to the associated metadata and inner exception for detailed information.";
        var exception = new AppException(error, ExceptionCode.Internal, e);

        if (response != null)
        {
            exception.AddData(response.GetSummary());
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