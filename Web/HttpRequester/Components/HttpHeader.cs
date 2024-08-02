using Aidan.Core;
using System.Collections;
using System.Text;
using System.Text.Json.Serialization;

namespace Aidan.Web.Http;

/// <summary>
/// Represents the Authorization header structure.
/// </summary>
public struct AuthorizationHeader
{
    public string Scheme { get; set; }
    public string Value { get; set; }

    public AuthorizationHeader(string scheme, string value)
    {
        Scheme = scheme;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Scheme} {Value}";
    }

    /// <summary>
    /// Creates an AuthorizationHeader with Bearer scheme.
    /// </summary>
    /// <param name="token">The bearer token value.</param>
    /// <returns>An AuthorizationHeader with Bearer scheme.</returns>
    public static AuthorizationHeader Bearer(string token)
    {
        return new AuthorizationHeader("Bearer", token);
    }
}

/// <summary>
/// Represents a collection of HTTP headers.
/// </summary>
public partial class HttpHeader : IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>
    /// Additional headers not explicitly defined.
    /// </summary>
    /// [JsonInclude]
    [JsonInclude, JsonPropertyName("Values")]
    private Dictionary<string, string> Values { get; set; } // Set made private to ensure integrity.

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpHeader"/> class.
    /// </summary>
    public HttpHeader()
    {
        Values = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpHeader"/> class by parsing headers from the provided <see cref="HttpResponseMessage"/>.
    /// </summary>
    /// <param name="responseMessage">The response message to parse headers from.</param>
    public HttpHeader(HttpResponseMessage responseMessage) : this()
    {
        foreach (var item in responseMessage.Headers)
        {
            var count = item.Value.Count();

            if (count == 0)
            {
                continue;
            }
            if (count == 1)
            {
                Values.Add(NormalizeKey(item.Key), item.Value.First());
            }
            if (count > 1)
            {
                var strBuilder = new StringBuilder();
                strBuilder.AppendJoin(", ", item.Value);
                Values.Add(NormalizeKey(item.Key), strBuilder.ToString());
            }
        }

        foreach (var item in responseMessage.Content.Headers)
        {
            var count = item.Value.Count();

            if (count == 0)
            {
                continue;
            }
            if (count == 1)
            {
                Values.Add(NormalizeKey(item.Key), item.Value.First());
            }
            if (count > 1)
            {
                var strBuilder = new StringBuilder();
                strBuilder.AppendJoin(", ", item.Value);
                Values.Add(NormalizeKey(item.Key), strBuilder.ToString());
            }
        }
    }

    public string? this[string key]
    {
        get
        {
            return Get(key);
        }
        set
        {
            Set(key, value);
        }
    }

    public static string NormalizeKey(string key)
    {
        return key.ToLower().Trim();
    }

    /// <summary>
    /// Returns a deep copy of the current HttpHeader.
    /// </summary>
    /// <returns>A new copy of HttpHeader.</returns>
    public HttpHeader Copy()
    {
        return new HttpHeader()
        {
            ContentType = ContentType,
            Authorization = Authorization,
            Values = new Dictionary<string, string>(Values)
        };
    }

    /// <summary>
    /// Checks if the header contains the specified key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if the header contains the key, otherwise false.</returns>
    public bool Contains(string key)
    {
        return Values.Keys.Any(x => x.ToLower() == key.ToLower());
    }

    /// <summary>
    /// Retrieves the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to retrieve.</param>
    /// <returns>The value associated with the key if found, otherwise null.</returns>
    public string? Get(string key)
    {
        var values = Values.Where(x => NormalizeKey(x.Key) == NormalizeKey(key));

        if (values.IsEmpty())
        {
            return null;
        }

        return values.First().Value;
    }

    public int? GetInt(string key)
    {
        if (int.TryParse(Get(key), out var parsed))
        {
            return parsed;
        }

        return null;
    }

    public long? GetLong(string key)
    {
        if (long.TryParse(Get(key), out var parsed))
        {
            return parsed;
        }

        return null;
    }

    public decimal? GetDecimal(string key)
    {
        if (decimal.TryParse(Get(key), out var parsed))
        {
            return parsed;
        }

        return null;
    }

    public AuthorizationHeader? GetAuthorization()
    {
        var header = Get("authorization");

        if (header == null)
        {
            return null;
        }

        var split = header.Split(' ');

        if (split.Length < 2)
        {
            return null;
        }

        var scheme = split[0];
        var value = header.Substring(scheme.Length);

        return new AuthorizationHeader(scheme, value);
    }

    /// <summary>
    /// Sets the value for the specified key in the header.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to associate with the key.</param>
    /// <returns>The current <see cref="HttpHeader"/> instance.</returns>
    public HttpHeader Set(string key, string? value)
    {
        if (value == null)
        {
            foreach (var _key in Values.Keys)
            {
                if (NormalizeKey(_key) == NormalizeKey(key))
                {
                    Values.Remove(_key);
                }
            }

            return this;
        }

        Values[NormalizeKey(key)] = value;
        return this;
    }

    public void SetInt(string key, int? value)
    {
        Set(key, value?.ToString());
    }

    public void SetLong(string key, long? value)
    {
        Set(key, value?.ToString());
    }

    public void SetDecimal(string key, decimal? value)
    {
        Set(key, value?.ToString());
    }

    /// <summary>
    /// Sets the authorization header with a bearer token.
    /// </summary>
    /// <param name="token">The token to set in the authorization header.</param>
    /// <returns>The current <see cref="HttpHeader"/> instance.</returns>
    public HttpHeader SetBearerToken(string token)
    {
        Authorization = AuthorizationHeader.Bearer(token);
        return this;
    }

    /// <summary>
    /// Combines the current <see cref="HttpHeader"/> instance with another <see cref="HttpHeader"/> instance.
    /// </summary>
    /// <param name="header">The <see cref="HttpHeader"/> instance to combine with the current instance.</param>
    /// <param name="useParamAsTarget">Determines which <see cref="HttpHeader"/> (current instance or provided instance) is the target for combination. If set to true, the provided header instance is the target; otherwise, the current instance is the target.</param>
    /// <param name="preserveTargetHeaders">Determines whether to preserve headers in the target that also exist in the source. If set to true, headers in the target are preserved and not overwritten; otherwise, they are overwritten by headers from the source.</param>
    /// <returns>The combined <see cref="HttpHeader"/> instance.</returns>
    /// <remarks>
    /// This method will combine headers from the source into the target. The source and target are determined based on the <paramref name="useParamAsTarget"/> parameter.
    /// The `Authorization` and `ContentType` headers are always combined, unless they are null in the source. Additional headers from the source are combined into the target unless <paramref name="preserveTargetHeaders"/> is true and the header already exists in the target.
    /// </remarks>
    public HttpHeader Combine(HttpHeader header, bool useParamAsTarget = false, bool preserveTargetHeaders = false)
    {
        var source = useParamAsTarget ? this : header;
        var target = useParamAsTarget ? header : this;

        if (source.Authorization != null)
        {
            target.Authorization = source.Authorization;
        }
        if (source.ContentType != null)
        {
            target.ContentType = source.ContentType;
        }

        foreach (var keyValue in source.Values)
        {
            if (preserveTargetHeaders && target.Contains(keyValue.Key))
            {
                continue;
            }

            target.Set(keyValue.Key, keyValue.Value);
        }

        return target;
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Values.GetEnumerator();
    }
}

//*
// Defines HTTP header properties.
//*

public partial class HttpHeader : IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>
    /// Gets or sets the accept header indicating media types that the client can process.
    /// </summary>
    public string? Accept
    {
        get => Get("accept");
        set => Set("accept", value);
    }

    /// <summary>
    /// Gets or sets the accept charset.
    /// </summary>
    public string? AcceptCharset
    {
        get => Get("accept-charset");
        set => Set("accept-charset", value);
    }

    /// <summary>
    /// Gets or sets the accept encoding.
    /// </summary>
    public string? AcceptEncoding
    {
        get => Get("accept-encoding");
        set => Set("accept-encoding", value);
    }

    /// <summary>
    /// Gets or sets the access control allow headers.
    /// </summary>
    public string? AccessControlAllowHeaders
    {
        get => Get("access-control-allow-headers");
        set => Set("access-control-allow-headers", value);
    }

    /// <summary>
    /// Gets or sets the access control allow methods.
    /// </summary>
    public string? AccessControlAllowMethods
    {
        get => Get("access-control-allow-methods");
        set => Set("access-control-allow-methods", value);
    }

    /// <summary>
    /// Gets or sets the access control allow origin.
    /// </summary>
    public string? AccessControlAllowOrigin
    {
        get => Get("access-control-allow-origin");
        set => Set("access-control-allow-origin", value);
    }

    /// <summary>
    /// Gets or sets the access control max age.
    /// </summary>
    public int? AccessControlMaxAge
    {
        get => GetInt("access-control-max-age");
        set => SetInt("access-control-max-age", value);
    }

    /// <summary>
    /// Gets or sets the allow header indicating what HTTP methods are allowed in a response.
    /// </summary>
    public string? Allow
    {
        get => Get("allow");
        set => Set("allow", value);
    }

    /// <summary>
    /// Gets or sets the authorization.
    /// </summary>
    public AuthorizationHeader? Authorization
    {
        get => GetAuthorization();
        set => Set("authorization", value.ToString());
    }

    /// <summary>
    /// Gets or sets the cache control.
    /// </summary>
    public string? CacheControl
    {
        get => Get("cache-control");
        set => Set("cache-control", value);
    }

    /// <summary>
    /// Gets or sets the connection management header.
    /// </summary>
    public string? Connection
    {
        get => Get("connection");
        set => Set("connection", value);
    }

    /// <summary>
    /// Gets or sets the 'Cookie' header, which contains stored HTTP cookies previously sent by the server with the Set-Cookie header.
    /// </summary>
    public string? Cookie
    {
        get => Get("cookie");
        set => Set("cookie", value);
    }

    /// <summary>
    /// Gets or sets the content disposition of the HTTP header.
    /// </summary>
    public string? ContentDisposition
    {
        get => Get("content-disposition");
        set => Set("content-disposition", value);
    }

    /// <summary>
    /// Gets or sets the content encoding of the HTTP header.
    /// </summary>
    public string? ContentEncoding
    {
        get => Get("content-encoding");
        set => Set("content-encoding", value);
    }

    /// <summary>
    /// Gets or sets the content length of the HTTP header.
    /// </summary>
    public long? ContentLength
    {
        get => GetLong("content-length");
        set => SetLong("content-length", value);
    }

    /// <summary>
    /// Gets or sets the content range of the HTTP header.
    /// </summary>
    public string? ContentRange
    {
        get => Get("content-range");
        set => Set("content-range", value);
    }

    /// <summary>
    /// Gets or sets the content type of the HTTP header.
    /// </summary>
    public string? ContentType
    {
        get => Get("content-type");
        set => Set("content-type", value);
    }

    /// <summary>
    /// Gets or sets the 'Set-Cookie' header, which is used to send cookies from the server to the user agent.
    /// </summary>
    public string? SetCookie
    {
        get => Get("set-cookie");
        set => Set("set-cookie", value);
    }

    /// <summary>
    /// Gets or sets the 'Server' header value.
    /// </summary>
    public string? Server
    {
        get => Get("server");
        set => Set("server", value);
    }

    /// <summary>
    /// Gets or sets the 'User-Agent' header value.
    /// </summary>
    public string? UserAgent
    {
        get => Get("user-agent");
        set => Set("user-agent", value);
    }

    /// <summary>
    /// Gets or sets the 'Range' header, which specifies the part of the document to be sent.
    /// </summary>
    public string? Range
    {
        get => Get("range");
        set => Set("range", value);
    }

    /// <summary>
    /// Gets or sets the 'Upgrade' header, which allows the client to specify what additional communication protocols it supports.
    /// </summary>
    public string? Upgrade
    {
        get => Get("upgrade");
        set => Set("upgrade", value);
    }

    /// <summary>
    /// Gets or sets the Vary header value.
    /// </summary>
    public string? Vary
    {
        get => Get("vary");
        set => Set("vary", value);
    }
}
