using ModularSystem.Core;
using ModularSystem.Web.Http;

namespace ModularSystem.Web;

/// <summary>
/// Defines a RESTful endpoint that processes input of type <typeparamref name="TIn"/> and produces output of type <typeparamref name="TOut"/>. <br/>
/// This interface represents a contract for implementing RESTful services where there is a clear input and output type.
/// </summary>
/// <typeparam name="TIn">The type of the input data expected by the endpoint.</typeparam>
/// <typeparam name="TOut">The type of the output data produced by the endpoint.</typeparam>
public interface IRestfulEndpoint<TIn, TOut>
{
    /// <summary>
    /// Asynchronously processes the provided input and produces an output. <br/>
    /// This method encapsulates the core logic for the RESTful endpoint, handling the incoming request and producing the appropriate response.
    /// </summary>
    /// <param name="input">The input data of type <typeparamref name="TIn"/> to be processed by the endpoint.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, resulting in an <see cref="OperationResult{TOut}"/>. <br/>
    /// The <see cref="OperationResult{TOut}"/> encapsulates the outcome of the operation, containing either the result of type <typeparamref name="TOut"/> if successful, or error information if the operation fails.
    /// </returns>
    Task<OperationResult<TOut>> RunAsync(TIn input);
}

/// <summary>
/// Represents the configuration settings for a RESTful endpoint.
/// </summary>
public class EndpointConfiguration
{
    /// <summary>
    /// Gets or sets the request URI for the endpoint.
    /// </summary>
    public URI RequestUri { get; set; }

    /// <summary>
    /// Gets or sets the header configuration for the endpoint.
    /// </summary>
    public HttpHeader? Header { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointConfiguration"/> class with the specified base URI.
    /// </summary>
    /// <param name="baseUri">The base URI for the endpoint.</param>
    public EndpointConfiguration(URI baseUri)
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

    /// <summary>
    /// Asynchronously processes the provided input and produces an output. <br/>
    /// Handles the entire request-response lifecycle including <br/> request creation, sending the request,
    /// receiving the response, and processing the response.
    /// </summary>
    /// <param name="input">The input data of type <typeparamref name="TIn"/> to be processed by the endpoint.</param>
    /// <returns>
    /// A task that represents the asynchronous operation, resulting in an <see cref="OperationResult{TOut}"/>. <br/>
    /// The <see cref="OperationResult{TOut}"/> encapsulates the outcome of the operation, containing either <br/>
    /// the result of type <typeparamref name="TOut"/> if successful, or error information if the operation fails.
    /// </returns>
    public virtual async Task<OperationResult<TOut>> RunAsync(TIn input)
    {
        HttpClient? httpClient = null;
        OperationResult<TOut>? result = null;
        TOut? outputData = default;

        HttpRequest? request = null;
        HttpResponse? response = null;

        HttpRequestMessage? requestMessage = null;
        HttpResponseMessage? responseMessage = null;

        try
        {
            httpClient = CreateRequester();
            request = CreateRequest(input);

            OnRequestCreated(request);
            await OnRequestCreatedAsync(request);

            requestMessage = request.ToHttpRequestMessage();
            responseMessage = await httpClient.SendAsync(requestMessage);
            response = responseMessage.ToHttpResponse(request);

            result = OnResponse(response);
            if(result != null) return result;

            result = await OnResponseAsync(response);
            if (result != null) return result;

            if (!response.IsSuccess)
            {
                return await HandleFailureResponseAsync(response);
            }

            result = BeforeDeserialize(response);
            if (result != null) return result;

            result = await BeforeDeserializeAsync(response);
            if (result != null) return result;

            outputData = await DeserializeResponseAsync(response);

            result = AfterDeserialize(response, outputData);
            if (result != null) return result;

            result = await AfterDeserializeAsync(response, outputData);
            if (result != null) return result;

            return new OperationResult<TOut>(outputData);
        }
        catch (Exception e)
        {
            throw await HandleExceptionAsync(e, request, response);
        }
        finally
        {
            requestMessage?.Dispose();
            responseMessage?.Dispose();
        }
    }

    /// <summary>
    /// Handles the failure response received from an HTTP endpoint. <br/>
    /// This method should be implemented to define how to process unsuccessful HTTP responses <br/> and convert them into an appropriate <see cref="OperationResult{TOut}"/>.
    /// </summary>
    /// <param name="response">The <see cref="HttpResponse"/> object representing the HTTP response from the endpoint.</param>
    /// <returns>A task representing the asynchronous operation, resulting in an <see cref="OperationResult{TOut}"/> that represents the failure.</returns>
    protected abstract Task<OperationResult<TOut>> HandleFailureResponseAsync(HttpResponse response);

    /// <summary>
    /// Retrieves an instance of the HttpRequester class to send HTTP requests.
    /// </summary>
    /// <returns>An instance of <see cref="HttpRequester"/>.</returns>
    protected virtual HttpClient CreateRequester()
    {
        return HttpClientSingleton.Value;
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
    protected virtual OperationResult<TOut>? OnResponse(HttpResponse response)
    {
        return null;
    }

    /// <summary>
    /// Asynchronously invoked after receiving the HttpResponse, regardless of its status code.
    /// </summary>
    /// <param name="response">The received HttpResponse.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task<OperationResult<TOut>?> OnResponseAsync(HttpResponse response)
    {
        return Task.FromResult<OperationResult<TOut>?>(null);
    }

    /// <summary>
    /// Invoked before the deserialization process of a successful response. <br/>
    /// This method provides an opportunity to perform any pre-processing or validation on the raw response data.
    /// </summary>
    /// <param name="response">The received HttpResponse that will be deserialized.</param>
    protected virtual OperationResult<TOut>? BeforeDeserialize(HttpResponse response)
    {
        return null;
    }

    /// <summary>
    /// Asynchronously invoked before the deserialization process of a successful response. <br/>
    /// This method provides an opportunity to perform any asynchronous pre-processing or validation on the raw response data.
    /// </summary>
    /// <param name="response">The received HttpResponse that will be deserialized.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task<OperationResult<TOut>?> BeforeDeserializeAsync(HttpResponse response)
    {
        return Task.FromResult<OperationResult<TOut>?>(null);
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
    /// <param name="data">The deserialized payload of type <typeparamref name="TOut"/>.</param>
    protected virtual OperationResult<TOut>? AfterDeserialize(HttpResponse response, TOut data)
    {
        return null;
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
    /// <param name="data">The deserialized payload of type <typeparamref name="TOut"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task<OperationResult<TOut>?> AfterDeserializeAsync(HttpResponse response, TOut data)
    {
        return Task.FromResult<OperationResult<TOut>?>(null);
    }

    /// <summary>
    /// Asynchronously deserializes the successful HTTP response into the specified output type <typeparamref name="TOut"/>. <br/>
    /// This method is responsible for converting the response body into a meaningful data structure that can be returned and processed by the caller.
    /// </summary>
    /// <remarks>
    /// Implement this method to provide the logic for converting the raw response body into the desired output type. <br/>
    /// This is a critical step in processing the response, as it transforms the raw data into a more usable and structured format. <br/>
    /// This method is typically called after a successful HTTP response is received, and the response body needs to be interpreted or processed.
    /// </remarks>
    /// <param name="response">The HttpResponse object representing the successful API response.</param>
    /// <returns>
    /// A task representing the asynchronous operation, which upon completion, yields the deserialized response of type <typeparamref name="TOut"/>.
    /// </returns>
    /// <exception cref="System.Text.Json.JsonException">
    /// Thrown if the JSON deserialization fails due to an invalid format, type mismatch, or other issues related to the content of the response.
    /// </exception>
    protected virtual Task<TOut> DeserializeResponseAsync(HttpResponse response)
    {
        return response.DeserializeAsJsonAsync<TOut>();
    }

    /// <summary>
    /// Handles exceptions that occur during the API request and response cycle. <br/>
    /// This method should be implemented to define custom exception handling logic, which might include logging, 
    /// <br/> custom error messages, and deciding whether to rethrow the exception.
    /// </summary>
    /// <param name="e">The original exception.</param>
    /// <param name="request">The HttpRequest object, if available.</param>
    /// <param name="response">The HttpResponse object, if available.</param>
    /// <returns>A task representing the asynchronous operation, resulting in an <see cref="Exception"/> that should be thrown or propagated.</returns>
    protected virtual async Task<Exception> HandleExceptionAsync(Exception e, HttpRequest? request, HttpResponse? response)
    {
        var error = $"Failed to process the HTTP API request. Refer to the associated metadata and inner exception for detailed information.";
        var exception = new AppException(error, ExceptionCode.Internal, e);

        if (response != null)
        {
            exception.AddData(await response.CreateSummaryCopyAsync());
        }
        else if (request != null)
        {
            exception.AddData(request);
        }

        return exception;
    }

}
