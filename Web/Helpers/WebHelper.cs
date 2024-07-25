namespace ModularSystem.Web;

/// <summary>
/// Provides helper methods to convert byte arrays to Base64 strings and vice versa, <br/>
/// with modifications to make the Base64 strings safe for URL usage.
/// </summary>
public static class WebHelper
{
    private const char PlusAlias = '-';
    private const char SlashAlias = '_';

    /// <summary>
    /// Converts a byte array to a URL-safe Base64 string.
    /// </summary>
    /// <param name="bytes">The byte array to convert to Base64.</param>
    /// <returns>A URL-safe Base64 string representation of the byte array.</returns>
    /// <remarks>
    /// This method modifies the standard Base64 encoding by replacing '+' with '-' <br/>
    /// and '/' with '_', and removing any trailing '=' characters. This makes the <br/>
    /// resulting string safe for use in URLs, including query parameters.
    /// </remarks>
    public static string ToUrlBase64(byte[] bytes)
    {
        return Convert
            .ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', PlusAlias)
            .Replace('/', SlashAlias);
    }

    /// <summary>
    /// Converts a URL-safe Base64 string back to a byte array.
    /// </summary>
    /// <param name="base64Text">The URL-safe Base64 string to convert.</param>
    /// <returns>A byte array represented by the Base64 string.</returns>
    /// <remarks>
    /// This method reverses the modifications made by <see cref="ToUrlBase64(byte[])"/>, <br/>
    /// replacing '-' with '+' and '_' with '/', and adding '=' padding if necessary. <br/>
    /// It is used to decode a URL-safe Base64 string back to its original byte array form.
    /// </remarks>
    public static byte[] FromUrlBase64(string base64Text)
    {
        var paddedBase64 = base64Text
            .Replace(PlusAlias, '+')
            .Replace(SlashAlias, '/');

        switch (paddedBase64.Length % 4) // Preenche com '=' se necessário
        {
            case 2: paddedBase64 += "=="; break;
            case 3: paddedBase64 += "="; break;
        }

        return Convert.FromBase64String(paddedBase64);
    }
}
