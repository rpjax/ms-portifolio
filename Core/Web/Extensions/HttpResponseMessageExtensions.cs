namespace ModularSystem.Web;

public static class HttpResponseMessageExtensions
{
    public static bool ContainsAuthorizationHeader(this HttpResponseMessage message)
    {
        if (message == null)
        {
            return false;
        }

        var authorizationHeaders = message.Headers
            .Where(x =>
                string.Equals(x.Key, "Authorization", StringComparison.OrdinalIgnoreCase) &&
                x.Value.Any(str => !string.IsNullOrEmpty(str))
            );

        return authorizationHeaders.Any();
    }

    public static string? TryGetAuthorizationHeader(this HttpResponseMessage message)
    {
        if (message == null)
        {
            return null;
        }

        // Try to get the header value in a case-insensitive manner
        var authorizationHeaders = message.Headers
            .Where(x =>
                string.Equals(x.Key, "Authorization", StringComparison.OrdinalIgnoreCase) &&
                x.Value.Any(str => !string.IsNullOrEmpty(str))
            )
            .ToArray();

        if (authorizationHeaders.Length == 0)
        {
            return null;
        }

        return authorizationHeaders[0].Value.FirstOrDefault();
    }

    public static string? TryGetAuthorizationHeaderSchema(this HttpResponseMessage message)
    {
        var authHeader = TryGetAuthorizationHeader(message);

        if (string.IsNullOrWhiteSpace(authHeader))
        {
            return null;
        }

        var parts = authHeader.Split(new[] { ' ' }, 2);

        return parts.Length > 0 ? parts[0] : null;
    }

    public static string? TryGetAuthorizationHeaderValue(this HttpResponseMessage message)
    {
        var authHeader = TryGetAuthorizationHeader(message);

        if (string.IsNullOrWhiteSpace(authHeader))
        {
            return null;
        }

        var parts = authHeader.Split(new[] { ' ' }, 2);

        return parts.Length > 1 ? parts[1] : null;
    }
}