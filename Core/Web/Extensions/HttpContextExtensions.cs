using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ModularSystem.Core;
using System.Text.RegularExpressions;

namespace ModularSystem.Web;

public static class HttpContextExtensions
{
    public static Regex BearerRegex = new Regex("(?i)bearer", RegexOptions.Compiled);

    public static string? GetQueryParam(this HttpContext context, string name)
    {
        if (context.Request.Query.TryGetValue(name, out StringValues stringValue))
        {
            return stringValue.FirstOrDefault();

        }

        return null;
    }

    public static string? GetCookie(this HttpContext context, string name)
    {
        if (context.Request.Cookies.TryGetValue(name, out string? cookie))
        {
            return cookie;
        }

        return null;
    }

    public static string? GetAuthorizationHeader(this HttpContext context)
    {
        var authorizationHeaderEnumerable = context.Request.Headers.Where(x => x.Key.ToLower() == "authorization");

        if (authorizationHeaderEnumerable.IsEmpty())
        {
            return null;
        }

        return authorizationHeaderEnumerable.First().Value;
    }

    public static string? GetBearerToken(this HttpContext context)
    {
        var authorizationHeader = context.GetAuthorizationHeader();

        if (authorizationHeader == null)
        {
            return null;
        }

        var shouldRemoveBearerWord = authorizationHeader.ToLower().Trim().StartsWith("bearer");

        if (shouldRemoveBearerWord)
        {
            var match = BearerRegex.Match(authorizationHeader);

            return authorizationHeader.Substring(match.Index + match.Length).Replace(" ", "");
        }

        return authorizationHeader;
    }
}
