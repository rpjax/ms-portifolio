namespace Aidan.Core;

/// <summary>
/// Represents a Uniform Resource Identifier (URI) and provides easy access and manipulation of the parts of the URI.
/// <br/>
/// This class provides a custom implementation that mimics the functionality of <see cref="System.Uri"/>, 
/// <br/>
/// facilitating the construction and modification of URIs for web requests within the application.
/// </summary>
public class URI
{
    /// <summary>
    /// Gets or sets the Port number of the URI.
    /// </summary>
    public int Port { get; set; } = 80;

    /// <summary>
    /// Gets or sets the Scheme of the URI, such as 'http' or 'https'.
    /// </summary>
    public string Scheme { get; set; } = "http";

    /// <summary>
    /// Gets or sets the Host name of the URI.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the Path of the URI.
    /// </summary>
    public string Path { get; set; } = "/";

    /// <summary>
    /// Gets or sets the Query Parameters of the URI in the form of a list of key-value pairs.
    /// </summary>
    public List<KeyValuePair<string, string>> QueryParams { get; set; } = new();

    /// <summary>
    /// Gets the original string used to construct the URI, if available.
    /// </summary>
    public string? OriginalString { get; private set; } = null;

    public URI()
    {

    }

    public URI(string uriString)
    {
        var uri = new System.Uri(uriString);

        Port = uri.Port;
        Scheme = uri.Scheme;
        Host = uri.Host;
        Path = uri.AbsolutePath;
        QueryParams = DecodeQueryString(uri.Query);
        OriginalString = uriString;
    }

    public URI(System.Uri uri)
    {
        Port = uri.Port;
        Scheme = uri.Scheme;
        Host = uri.Host;
        Path = uri.AbsolutePath;
        QueryParams = DecodeQueryString(uri.Query);
        OriginalString = uri.OriginalString;
    }

    public static implicit operator URI(string uri)
    {
        return new URI(uri);
    }

    public static implicit operator URI(System.Uri uri)
    {
        return new URI(uri);
    }

    public static implicit operator System.Uri(URI uri)
    {
        return uri.ToSystemUri();
    }

    public override string ToString()
    {
        return $"{Scheme}://{Host}:{Port}/{GetNormalizedPath()}{GetQueryString()}";
    }

    public System.Uri ToSystemUri()
    {
        return new System.Uri(ToString());
    }

    public URI Copy()
    {
        return new URI(ToSystemUri());
    }

    public string GetQueryString()
    {
        var str = "";
        var isFirst = true;

        foreach (var param in QueryParams)
        {
            if (isFirst)
            {
                str = str + $"?{param.Key}={param.Value}";
            }
            else
            {
                str = str + $"&{param.Key}={param.Value}";
            }

            isFirst = false;
        }

        return str;
    }

    public string? GetQueryParam(string key)
    {
        foreach (var item in QueryParams)
        {
            if (item.Key == key)
            {
                return item.Value;
            }
        }

        return null;
    }

    public URI SetQueryParam(string key, string? value)
    {
        if (value == null)
        {
            return this;
        }

        QueryParams.RemoveAll(x => x.Key == key);
        QueryParams.Add(new KeyValuePair<string, string>(key, value));
        return this;
    }

    public URI SetQueryParam(string key, object? value)
    {
        return SetQueryParam(key, value?.ToString());
    }

    public URI SetQueryParam(string key, IEnumerable<string?> values)
    {
        QueryParams.RemoveAll(x => x.Key == key);

        foreach (var value in values)
        {
            if (value == null)
            {
                continue;
            }

            QueryParams.Add(new KeyValuePair<string, string>(key, value));
        }

        return this;
    }

    public URI AppendPath(string path)
    {
        var originalPath = Path;

        if (path.StartsWith('/'))
        {
            path = path.Substring(1);
        }

        if (originalPath.EndsWith('/'))
        {
            Path = $"{originalPath}{path}";
        }
        else
        {
            Path = $"{originalPath}/{path}";
        }

        return this;
    }

    string GetNormalizedPath()
    {
        if (Path.StartsWith('/'))
        {
            Path = Path.Substring(1);
        }
        return Path;
    }

    List<KeyValuePair<string, string>> DecodeQueryString(string queryString)
    {
        var values = new List<KeyValuePair<string, string>>();
        var queryDictionary = System.Web.HttpUtility.ParseQueryString(queryString);

        foreach (var key in queryDictionary.AllKeys)
        {
            values.Add(new KeyValuePair<string, string>(key ?? string.Empty, queryDictionary.Get(key) ?? string.Empty));
        }

        return values;
    }
}
