using ModularSystem.Core;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ModularSystem.Web.Http;

/// <summary>
/// Represents an abstract body of an HTTP request.
/// </summary>
public abstract class HttpRequestBody
{
    /// <summary>
    /// Gets or sets the content of the HTTP request body.
    /// </summary>
    public byte[] Content { get; set; }

    protected HttpRequestBody()
    {
        Content = new byte[0];
    }

    /// <summary>
    /// Gets the MIME content type of the HTTP request body.
    /// </summary>
    /// <returns>The content type as a string.</returns>
    public abstract string ContentType();

    /// <summary>
    /// Converts the body content to an instance of <see cref="HttpContent"/>.
    /// </summary>
    /// <returns>The HTTP content.</returns>
    public abstract HttpContent ToHttpContent();
}

/// <summary>
/// Represents the body of an HTTP response.
/// </summary>
public class HttpResponseBody
{
    /// <summary>
    /// Gets the underlying HTTP response message associated with this body.
    /// </summary>
    protected HttpResponseMessage ResponseMessage { get; }

    public HttpResponseBody(HttpResponseMessage responseMessage)
    {
        ResponseMessage = responseMessage;
    }

    public override string ToString()
    {
        return ReadAsString(Encoding.UTF8);
    }

    /// <summary>
    /// Retrieves the underlying HTTP response message.
    /// </summary>
    /// <returns>The HTTP response message.</returns>
    public HttpResponseMessage? GetResponseMessage()
    {
        return ResponseMessage;
    }

    /// <summary>
    /// Reads the body as a byte array.
    /// </summary>
    /// <returns>The byte array.</returns>
    public byte[] ReadAsBytes()
    {
        if (ResponseMessage == null)
        {
            throw new InvalidOperationException();
        }

        byte[] bytes;
        var stream = ResponseMessage.Content.ReadAsStream();
        using var memoryStream = new MemoryStream();
        using var binaryReader = new BinaryReader(memoryStream);

        stream.CopyTo(memoryStream);
        bytes = binaryReader.ReadBytes((int)stream.Length);

        return bytes;
    }

    /// <summary>
    /// Asynchronously reads the body as a byte array.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The value of the TResult parameter contains the byte array.</returns>
    public Task<byte[]> ReadAsBytesAsync()
    {
        if (ResponseMessage == null)
        {
            throw new InvalidOperationException();
        }

        return ResponseMessage.Content.ReadAsByteArrayAsync();
    }

    /// <summary>
    /// Reads the body as a string using the specified encoding.
    /// </summary>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The decoded string.</returns>
    public string ReadAsString(Encoding? encoding = null)
    {
        if (encoding == null)
        {
            return ResponseMessage.Content.ReadAsStringAsync().Result;
        }

        return encoding.GetString(ReadAsBytes());
    }

    /// <summary>
    /// Asynchronously reads the body as a string using the specified encoding.
    /// </summary>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>A task representing the asynchronous operation. The value of the TResult parameter contains the decoded string.</returns>
    public async Task<string> ReadAsStringAsync(Encoding encoding)
    {
        return encoding.GetString(await ReadAsBytesAsync());
    }

    /// <summary>
    /// Reads the HTTP response content as a <see cref="Stream"/>.
    /// </summary>
    /// <returns>A <see cref="Stream"/> representing the HTTP response content.</returns>
    public Stream ReadAsStream()
    {
        return ResponseMessage.Content.ReadAsStream();
    }

    /// <summary>
    /// Asynchronously reads the HTTP response content as a <see cref="Stream"/>.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with a <see cref="Stream"/> result representing the HTTP response content.</returns>
    public Task<Stream> ReadAsStreamAsync()
    {
        return ResponseMessage.Content.ReadAsStreamAsync();
    }
}

/// <summary>
/// Represents a JSON body for HTTP requests.
/// </summary>
public class JsonBody : HttpRequestBody
{
    /// <summary>
    /// Gets or sets the encoding used for the JSON content.
    /// </summary>
    protected Encoding Encoding { get; set; }

    public JsonBody(object data)
    {
        Encoding = Encoding.UTF8;
        Content = EncodeToBytes(Serialize(data));
    }

    /// <summary>
    /// Serializes the provided data into a JSON string.
    /// </summary>
    protected virtual string Serialize(object data, JsonSerializerOptions? options = null)
    {
        return JsonSerializerSingleton.Serialize(data, options);
    }

    /// <summary>
    /// Encodes the provided JSON string into a byte array using the specified encoding.
    /// </summary>
    protected virtual byte[] EncodeToBytes(string json)
    {
        return Encoding.GetBytes(json);
    }

    public override HttpContent ToHttpContent()
    {
        return new StringContent(Encoding.GetString(Content), Encoding, "application/json");
    }

    public override string ContentType()
    {
        return "application/json";
    }
}

/// <summary>
/// Represents a body for HTTP requests with content type of "application/x-www-form-urlencoded".
/// </summary>
public class UrlEncodedFormDataBody : HttpRequestBody
{
    /// <summary>
    /// Gets or sets the key-value pairs for the form data.
    /// </summary>
    protected List<KeyValuePair<string, string>> Data { get; set; }

    public UrlEncodedFormDataBody(IEnumerable<KeyValuePair<string, string>>? data = null)
    {
        Data = data?.ToList() ?? new();
    }

    /// <summary>
    /// Adds a key-value pair to the form data.
    /// </summary>
    public UrlEncodedFormDataBody Add(string key, string value)
    {
        Data.Add(new KeyValuePair<string, string>(key, value));
        return this;
    }

    public override HttpContent ToHttpContent()
    {
        var content = new FormUrlEncodedContent(Data);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        return content;
    }

    public override string ContentType()
    {
        return "application/x-www-form-urlencoded";
    }
}