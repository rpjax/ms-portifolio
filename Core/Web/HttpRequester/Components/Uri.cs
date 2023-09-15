namespace ModularSystem.Web.Http;

/// <summary>
/// Uri facade.
/// </summary>
public class Uri
{
    public int Port { get; set; } = 80;
    public string Scheme { get; set; } = "http";
    public string Host { get; set; } = "localhost";
    public string Path { get; set; } = "";
    public string OriginalString { get; private set; } = string.Empty;
    public List<KeyValuePair<string, string>> QueryParams { get; set; }

    public Uri()
    {
        QueryParams = new List<KeyValuePair<string, string>>();
    }

    public Uri(string uriString) : this()
    {
        var uri = new System.Uri(uriString);

        Port = uri.Port;
        Scheme = uri.Scheme;
        Host = uri.Host;
        Path = uri.AbsolutePath;
        OriginalString = uriString;
        QueryParams = DecodeQueryString(uri.Query);
    }

    public Uri(System.Uri uri) : this()
    {
        Port = uri.Port;
        Scheme = uri.Scheme;
        Host = uri.Host;
        Path = uri.AbsolutePath;
        OriginalString = uri.OriginalString;
        QueryParams = DecodeQueryString(uri.Query);
    }

    public static implicit operator Uri(string uri)
    {
        return new Uri(uri);
    }

    public static implicit operator Uri(System.Uri uri)
    {
        return new Uri(uri);
    }

    public static implicit operator System.Uri(Uri uri)
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

    public Uri Copy()
    {
        return new Uri(ToSystemUri());
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

    public Uri SetQueryParam(string key, string? value)
    {
        if (value == null)
        {
            return this;
        }

        QueryParams.RemoveAll(x => x.Key == key);
        QueryParams.Add(new KeyValuePair<string, string>(key, value));
        return this;
    }

    public Uri SetQueryParam(string key, IEnumerable<string?> values)
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

    public Uri AppendPath(string path)
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
