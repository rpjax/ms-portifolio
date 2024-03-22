using ModularSystem.Core;
using ModularSystem.Core.Cryptography;

namespace ModularSystem.Web.Authentication;

///// <summary>
///// Defines an interface for a secure web token which can act as a bearer for credentials or claims.
///// </summary>
//public interface IToken
//{
//    /// <summary>
//    /// Gets or sets the prefix salt used in the token for added security.
//    /// </summary>
//    string? PrefixSalt { get; set; }

//    /// <summary>
//    /// Gets or sets the main payload of the token.
//    /// </summary>
//    string? Payload { get; set; }

//    /// <summary>
//    /// Gets or sets the suffix salt used in the token for added security.
//    /// </summary>
//    string? SuffixSalt { get; set; }

//    /// <summary>
//    /// Gets or sets the creation date and time of the token.
//    /// </summary>
//    DateTime CreatedAt { get; set; }

//    /// <summary>
//    /// Gets or sets the expiration date and time of the token.
//    /// </summary>
//    DateTime ExpiresAt { get; set; }

//    /// <summary>
//    /// Checks if the token has expired based on the current time.
//    /// </summary>
//    /// <returns><c>true</c> if the token has expired, otherwise <c>false</c>.</returns>
//    bool IsExpired();
//}

/// <summary>
/// Default implementation of a JSON token which provides a structure for a secure web token.
/// </summary>
public class Token
{
    /// <summary>
    /// Gets or sets the prefix salt used in the token for added security.
    /// </summary>
    public string? PrefixSalt { get; set; } = null;

    /// <summary>
    /// Gets or sets the main payload of the token.
    /// </summary>
    public string? Payload { get; set; } = null;

    /// <summary>
    /// Gets or sets the suffix salt used in the token for added security.
    /// </summary>
    public string? SuffixSalt { get; set; } = null;

    /// <summary>
    /// Gets or sets the creation date and time of the token.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the expiration date and time of the token.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Initializes a new instance of token with <see cref="CreatedAt"/> and <see cref="ExpiresAt"/> set to <see cref="TimeProvider.UtcNow"/>.
    /// </summary>
    public Token()
    {
        CreatedAt = TimeProvider.UtcNow();
        ExpiresAt = TimeProvider.UtcNow();
    }

    public static Token FromPayload(string payload, TimeSpan lifetime)
    {
        var random = new Random();
        var saltGenerator = new SaltGenerator();
        var prefixSaltSize = random.Next(20, 50);
        var suffixSaltSize = random.Next(20, 50);
        var prefixSalt = saltGenerator.Generate(prefixSaltSize);
        var suffixSalt = saltGenerator.Generate(suffixSaltSize);
        var now = TimeProvider.UtcNow();

        return new Token()
        {
            PrefixSalt = prefixSalt,
            Payload = payload,
            SuffixSalt = suffixSalt,
            CreatedAt = now,
            ExpiresAt = now.Add(lifetime)
        };
    }

    /// <summary>
    /// Checks if the token has expired by comparing its <see cref="ExpiresAt"/> property with the current time.
    /// </summary>
    /// <returns><c>true</c> if the <see cref="ExpiresAt"/> is earlier than the current time, otherwise <c>false</c>.</returns>
    public bool IsExpired()
    {
        return ExpiresAt < TimeProvider.Now();
    }
}
