using ModularSystem.Core.Cryptography;

namespace ModularSystem.Web.AccessManagement.Tokens;

/// <summary>
/// Defines an interface for a secure web token which can act as a bearer for credentials or claims.
/// </summary>
public interface IWebToken
{
    /// <summary>
    /// Gets the lifetime of the token.
    /// </summary>
    /// <returns></returns>
    TimeSpan GetLifetime();

    /// <summary>
    /// Gets the payload of the token.
    /// </summary>
    /// <returns>The payload of the token.</returns>
    string? GetPayload();
}

/// <summary>
/// Default implementation of a JSON token which provides a structure for a secure web token.
/// </summary>
public class WebToken : IWebToken
{
    /// <summary>
    /// Gets or sets the prefix salt used in the token for added security.
    /// </summary>
    public string? PrefixSalt { get; set; }

    /// <summary>
    /// Gets or sets the main payload of the token.
    /// </summary>
    public string? Payload { get; set; } 

    /// <summary>
    /// Gets or sets the suffix salt used in the token for added security.
    /// </summary>
    public string? SuffixSalt { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time of the token.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the expiration date and time of the token.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebToken"/> class.
    /// </summary>
    /// <param name="payload">The main payload of the token.</param>
    /// <param name="lifetime">The lifetime of the token.</param>
    /// <param name="createdAt">The creation date and time of the token. If not specified, the current UTC time will be used.</param>
    /// <param name="prefixSaltSize">The size of the prefix salt used in the token for added security. If not specified, a random value between 15 and 30 will be used.</param>
    /// <param name="suffixSaltSize">The size of the suffix salt used in the token for added security. If not specified, a random value between 15 and 50 will be used.</param>
    public WebToken(
        string? payload,
        TimeSpan lifetime,
        DateTime? createdAt = null,
        int? prefixSaltSize = null,
        int? suffixSaltSize = null)
    {
        var random = new Random();
        prefixSaltSize ??= random.Next(15, 30);
        suffixSaltSize ??= random.Next(15, 50);

        var saltGenerator = new SaltGenerator();
        var prefixSalt = saltGenerator.Generate(prefixSaltSize.Value);
        var suffixSalt = saltGenerator.Generate(suffixSaltSize.Value);

        PrefixSalt = prefixSalt;
        Payload = payload;
        SuffixSalt = suffixSalt;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        ExpiresAt = CreatedAt.Add(lifetime);
    }

    /// <summary>
    /// Checks if the token has expired by comparing its <see cref="ExpiresAt"/> property with the current time.
    /// </summary>
    /// <returns><c>true</c> if the <see cref="ExpiresAt"/> is earlier than the current time, otherwise <c>false</c>.</returns>
    public bool IsExpired()
    {
        return ExpiresAt < DateTime.UtcNow;
    }

    public TimeSpan GetLifetime()
    {
        return ExpiresAt - CreatedAt;
    }

    public string? GetPayload()
    {
        return Payload;
    }
}
