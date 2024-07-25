namespace ModularSystem.Core;

public static class UriExtensions
{
    public static Uri AppendPath(this Uri uri, string path)
    {
        var builder = new UriBuilder(uri);
        var originalPath = uri.AbsolutePath;

        if (path.StartsWith('/'))
        {
            path = path.Substring(1);
        }

        if (originalPath.EndsWith('/'))
        {
            builder.Path = $"{originalPath}{path}";
        }
        else
        {
            builder.Path = $"{originalPath}/{path}";
        }

        return builder.Uri;
    }
}
