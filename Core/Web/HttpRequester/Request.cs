using ModularSystem.Web.Http;

namespace ModularSystem.Web;

/// <summary>
/// Represents an HTTP request with its properties including URI, headers, method, and body.
/// </summary>
public class HttpRequest
{
    /// <summary>
    /// Gets or sets the HTTP method for the request.
    /// </summary>
    public HttpMethod Method { get; set; }

    /// <summary>
    /// Gets or sets the URI for the request.
    /// </summary>
    public Http.Uri Uri { get; set; }

    /// <summary>
    /// Gets or sets the headers associated with the request.
    /// </summary>
    public HttpHeader Header { get; set; }

    /// <summary>
    /// Gets or sets the request body. Can be null.
    /// </summary>
    public HttpRequestBody? Body { get => _body; set => SetBody(value); }

    private HttpRequestBody? _body = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequest"/> class with default values.
    /// </summary>
    public HttpRequest()
    {
        Uri = new Http.Uri();
        Header = new HttpHeader();
        Body = null;
        Method = HttpMethod.Get;
    }

    public HttpRequest(Http.Uri uri)
    {
        Uri = uri;
        Header = new HttpHeader();
        Body = null;
        Method = HttpMethod.Get;
    }

    public HttpRequest(Http.Uri uri, HttpMethod method)
    {
        Uri = uri;
        Header = new HttpHeader();
        Body = null;
        Method = method;
    }

    public HttpRequest(Http.Uri uri, HttpMethod method, HttpRequestBody body)
    {
        Uri = uri;
        Header = new HttpHeader();
        Body = body;
        Method = method;
    }

    public HttpRequest(Http.Uri uri, HttpMethod method, HttpHeader header, HttpRequestBody body)
    {
        Uri = uri;
        Header = header;
        Body = body;
        Method = method;
    }

    /// <summary>
    /// Sets the header for the HTTP request.
    /// </summary>
    /// <param name="key">The header key to set.</param>
    /// <param name="value">The value for the header key.</param>
    /// <returns>The current <see cref="HttpRequest"/> instance, allowing for method chaining.</returns>
    public HttpRequest SetHeader(string key, string value)
    {
        Header.Set(key, value);
        return this;
    }

    /// <summary>
    /// Sets the bearer token to be used in the Authorization header of the HTTP request.
    /// </summary>
    /// <param name="token">The bearer token value.</param>
    /// <returns>The current <see cref="HttpRequest"/> instance, allowing for method chaining.</returns>
    public HttpRequest SetBearerToken(string token)
    {
        Header.SetBearerToken(token);
        return this;
    }

    /// <summary>
    /// Sets a specific query parameter for the request's URI.
    /// </summary>
    /// <param name="key">The key of the query parameter.</param>
    /// <param name="value">The value of the query parameter.</param>
    /// <returns>The current <see cref="HttpRequest"/> instance, allowing for method chaining.</returns>
    public HttpRequest SetQueryParam(string key, string? value)
    {
        Uri.SetQueryParam(key, value);
        return this;
    }

    public HttpRequest SetQueryParams(string key, IEnumerable<string?> values)
    {
        Uri.SetQueryParam(key, values);
        return this;
    }

    public HttpRequest SetBody(HttpRequestBody? body)
    {
        Header.ContentType = body?.ContentType();
        _body = body;
        return this;
    }

    /// <summary>
    /// Sets the body of the HTTP request by serializing a given object into a JSON format.
    /// </summary>
    /// <param name="data">The object to be serialized to JSON and used as the request body.</param>
    /// <returns>The current <see cref="HttpRequest"/> instance, allowing for method chaining.</returns>
    public HttpRequest SetJsonBody(object data)
    {
        Body = new JsonBody(data);
        return this;
    }

    /// <summary>
    /// Converts the current <see cref="HttpRequest"/> into an <see cref="HttpRequestMessage"/> suitable for the System.Net.Http namespace.
    /// </summary>
    /// <returns>An <see cref="HttpRequestMessage"/> representation of the current request.</returns>
    public HttpRequestMessage ToHttpRequestMessage()
    {
        var request = new HttpRequestMessage();

        AddHeadersTo(ref request);

        request.RequestUri = Uri.ToSystemUri();
        request.Content = Body?.ToHttpContent();
        request.Method = Method;

        return request;
    }

    /// <summary>
    /// Adds headers from the current <see cref="HttpRequest"/> instance to the provided <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <param name="request">The target HttpRequestMessage to add headers to.</param>
    private void AddHeadersTo(ref HttpRequestMessage request)
    {
        foreach (var header in Header)
        {
            if (IsContentHeader(header.Key))
            {
                request.Content?.Headers.Add(header.Key, header.Value);
            }
            else
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        if (Header.Authorization != null)
        {
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(Header.Authorization.Value.Scheme, Header.Authorization?.Value);
        }
    }

    private bool IsContentHeader(string key)
    {
        return HttpRequestHelper.ContentHeaders.Contains(key);
    }
}

public static class HttpRequestHelper
{
    public static readonly HashSet<string> ContentHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Content-Length",
        "Content-Type",
        "Content-Encoding",
        "Content-Language",
        "Content-Location",
        "Content-Disposition",
        "Content-MD5",
        "Content-Range",
        "Content-Transfer-Encoding"
    };
}