﻿using ModularSystem.Core;
using ModularSystem.Web.Http;
using System.Text;
using System.Text.Json;

namespace ModularSystem.Web;

/// <summary>
/// Represents an HTTP response and provides methods to interact with and process its content.
/// </summary>
public class HttpResponse : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the HTTP response was successful.
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Gets the HTTP status code of the response.
    /// </summary>
    public int StatusCode { get; private set; }

    /// <summary>
    /// Gets or sets the HTTP headers associated with the response.
    /// </summary>
    public HttpHeader Header { get; set; } = new();

    /// <summary>
    /// Gets or sets the body of the HTTP response.
    /// </summary>
    public HttpResponseBody? Body { get; set; }

    /// <summary>
    /// Gets the corresponding HTTP request for the response.
    /// </summary>
    public HttpRequest? Request { get; }

    /// <summary>
    /// Gets or sets the raw HttpResponseMessage.
    /// </summary>
    public HttpResponseMessage ResponseMessage { get; set; }

    /// <summary>
    /// Gets or sets the options used when deserializing JSON content.
    /// </summary>
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpResponse"/> class.
    /// </summary>
    /// <param name="request">The corresponding HTTP request.</param>
    /// <param name="responseMessage">The raw HttpResponseMessage.</param>
    public HttpResponse(HttpRequest request, HttpResponseMessage responseMessage)
    {
        IsSuccess = responseMessage.IsSuccessStatusCode;
        StatusCode = (int)responseMessage.StatusCode;
        Header = new HttpHeader(responseMessage);
        Body = new HttpResponseBody(responseMessage);
        Request = request;
        ResponseMessage = responseMessage;
        JsonSerializerOptions = null;
    }

    /// <summary>
    /// Releases the resources used by the HttpResponse, specifically the HttpResponseMessage.
    /// </summary>
    public void Dispose()
    {
        ResponseMessage?.Dispose();
    }

    /// <summary>
    /// Reads the response body as a string using the provided encoding.
    /// </summary>
    /// <param name="encoding">The encoding to use when reading the response body.</param>
    /// <returns>The response body as a string.</returns>
    public string ReadAsString(Encoding? encoding = null)
    {
        if (Body == null)
        {
            return string.Empty;
        }

        return Body.ReadAsString(encoding);
    }

    /// <summary>
    /// Reads the response body asynchronously as a string using the provided encoding.
    /// </summary>
    /// <param name="encoding">The encoding to use when reading the response body.</param>
    /// <returns>A task representing the asynchronous operation with the response body as a string.</returns>
    public Task<string> ReadAsStringAsync(Encoding encoding)
    {
        if (Body == null)
        {
            return Task.FromResult(string.Empty);
        }

        return Body.ReadAsStringAsync(encoding);
    }

    /// <summary>
    /// Reads the response body as a stream.
    /// </summary>
    /// <returns>The response body as a stream.</returns>
    public Stream ReadAsStream()
    {
        if (Body == null)
        {
            return string.Empty.ToMemoryStream();
        }

        return Body.ReadAsStream();
    }

    //*
    // sync deserialziation
    //*

    /// <summary>
    /// Attempts to deserialize the response content from JSON into the specified object type using the provided encoding and options.
    /// </summary>
    /// <param name="type">The type of object to deserialize into.</param>
    /// <param name="encoding">The encoding used for the response content.</param>
    /// <param name="options">Optional parameters for the JSON deserializer.</param>
    /// <returns>The deserialized object if successful; otherwise, null.</returns>
    public object? TryDeserializeAsJson(Type type, Encoding? encoding = null, JsonSerializerOptions? options = null)
    {
        try
        {
            options ??= options ??= JsonSerializerOptions ??= DefaultJsonSerializerOptions();
            return JsonSerializerSingleton.Deserialize(Body.ReadAsString(encoding), type, options);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize the response content from JSON into the specified generic type using the provided encoding and options.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize into.</typeparam>
    /// <param name="encoding">The encoding used for the response content.</param>
    /// <param name="options">Optional parameters for the JSON deserializer.</param>
    /// <returns>The deserialized object if successful; otherwise, null.</returns>
    public T? TryDeserializeAsJson<T>(Encoding? encoding = null, JsonSerializerOptions? options = null) where T : class
    {
        try
        {
            return TryDeserializeAsJson(typeof(T), encoding, options)?.TypeCast<T>();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Deserializes the response content from JSON into the specified object type using the provided encoding and options. <br/>
    /// Throws an exception if deserialization fails.
    /// </summary>
    /// <param name="type">The type of object to deserialize into.</param>
    /// <param name="encoding">The encoding used for the response content.</param>
    /// <param name="options">Optional parameters for the JSON deserializer.</param>
    /// <returns>The deserialized object.</returns>
    public object DeserializeAsJson(Type type, Encoding? encoding = null, JsonSerializerOptions? options = null)
    {
        try
        {
            return TryDeserializeAsJson(type, encoding, options)!;
        }
        catch (Exception e)
        {
            throw new AppException($"Failed to deserialize the HTTP response content to an object of type '{type.FullName}'. Ensure that the response content is a valid JSON representation of the specified type.", ExceptionCode.Internal, e, this);
        }
    }

    /// <summary>
    /// Deserializes the response content from JSON into the specified generic type using the provided encoding and options.
    /// <br/>Throws an exception if deserialization fails.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize into.</typeparam>
    /// <param name="encoding">The encoding used for the response content.</param>
    /// <param name="options">Optional parameters for the JSON deserializer.</param>
    /// <returns>The deserialized object.</returns>
    public T DeserializeAsJson<T>(Encoding? encoding = null, JsonSerializerOptions? options = null) where T : class
    {
        try
        {
            return DeserializeAsJson(typeof(T), encoding, options)!.TypeCast<T>();
        }
        catch (Exception e)
        {
            throw new AppException($"Failed to deserialize the HTTP response content to an object of type '{typeof(T).FullName}'. Ensure that the response content is a valid JSON representation of the specified type.", ExceptionCode.Internal, e, this);
        }
    }

    //*
    // async deserialziation
    //*

    /// <summary>
    /// Attempts to asynchronously deserialize the response content from JSON into the specified object type using the provided encoding and options.
    /// </summary>
    /// <param name="type">The type of object to deserialize into.</param>
    /// <param name="encoding">The encoding used for the response content.</param>
    /// <param name="options">Optional parameters for the JSON deserializer.</param>
    /// <returns>A task representing the asynchronous operation with the deserialized object as the result. Returns null if deserialization fails.</returns>
    public async Task<object?> TryDeserializeAsJsonAsync(Type type, Encoding? encoding = null, JsonSerializerOptions? options = null)
    {
        try
        {
            using var stream = Body.ReadAsStream();
            options ??= options ??= JsonSerializerOptions ??= DefaultJsonSerializerOptions();

            return await JsonSerializerSingleton.DeserializeAsync(stream, type, options);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to asynchronously deserialize the response content from JSON into the specified generic type using the provided encoding and options.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize into.</typeparam>
    /// <param name="encoding">The encoding used for the response content.</param>
    /// <param name="options">Optional parameters for the JSON deserializer.</param>
    /// <returns>A task representing the asynchronous operation with the deserialized object as the result. Returns null if deserialization fails.</returns>
    public async Task<T?> TryDeserializeAsJsonAsync<T>(Encoding? encoding = null, JsonSerializerOptions? options = null) where T : class
    {
        try
        {
            return (await TryDeserializeAsJsonAsync(typeof(T), encoding, options))?.TypeCast<T>();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Asynchronously deserializes the response content from JSON into the specified object type using the provided encoding and options. Throws an exception if deserialization fails.
    /// </summary>
    /// <param name="type">The type of object to deserialize into.</param>
    /// <param name="encoding">The encoding used for the response content.</param>
    /// <param name="options">Optional parameters for the JSON deserializer.</param>
    /// <returns>A task representing the asynchronous operation with the deserialized object as the result.</returns>
    public Task<object> DeserializeAsJsonAsync(Type type, Encoding? encoding = null, JsonSerializerOptions? options = null)
    {
        try
        {
            return TryDeserializeAsJsonAsync(type, encoding, options)!;
        }
        catch (Exception e)
        {
            throw new AppException($"Failed to deserialize the HTTP response content to an object of type '{type.FullName}'. Ensure that the response content is a valid JSON representation of the specified type.", ExceptionCode.Internal, e, this);
        }
    }

    /// <summary>
    /// Asynchronously deserializes the response content from JSON into the specified generic type using the provided encoding and options. Throws an exception if deserialization fails.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize into.</typeparam>
    /// <param name="encoding">The encoding used for the response content.</param>
    /// <param name="options">Optional parameters for the JSON deserializer.</param>
    /// <returns>A task representing the asynchronous operation with the deserialized object as the result.</returns>
    public async Task<T> DeserializeAsJsonAsync<T>(Encoding? encoding = null, JsonSerializerOptions? options = null) where T : class
    {
        try
        {
            return (await DeserializeAsJsonAsync(typeof(T), encoding, options))!.TypeCast<T>();
        }
        catch (Exception e)
        {
            throw new AppException($"Failed to deserialize the HTTP response content to an object of type '{typeof(T).FullName}'. Ensure that the response content is a valid JSON representation of the specified type.", ExceptionCode.Internal, e, this);
        }
    }

    /// <summary>
    /// Retrieves a summary of the current HttpResponse instance, that is serializable and does not require disposing.
    /// </summary>
    /// <returns>A <see cref="SummaryCopy"/> instance representing a condensed version of the HTTP response.</returns>
    public SummaryCopy GetSummary()
    {
        return new SummaryCopy(this);
    }

    /// <summary>
    /// Gets the default JsonSerializerOptions for the HttpResponse.
    /// </summary>
    /// <remarks>
    /// This method sets the naming policy to use camel casing for JSON property names.
    /// </remarks>
    /// <returns>The default JsonSerializerOptions.</returns>
    protected JsonSerializerOptions DefaultJsonSerializerOptions()
    {
        return JsonSerializerSingleton.GetOptions(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
    }

    /// <summary>
    /// Represents a condensed or summarized version of an HTTP response, this object is serializable and does not require disposing.
    /// </summary>
    public class SummaryCopy
    {
        public bool IsSuccess { get; private set; }
        public int StatusCode { get; private set; }
        public HttpHeader Header { get; set; } = new();
        public string? Body { get; set; }
        public HttpRequest? Request { get; }

        public SummaryCopy(HttpResponse response)
        {
            IsSuccess = response.IsSuccess;
            StatusCode = response.StatusCode;
            Header = response.Header;
            Body = response?.Body.ReadAsString();
            Request = response.Request;
        }
    }
}