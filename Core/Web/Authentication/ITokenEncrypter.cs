using ModularSystem.Core;
using ModularSystem.Core.Cryptography;
using System.Net;
using System.Text;

namespace ModularSystem.Web.Authentication;

/// <summary>
/// Defines a set of methods for handling token encryption, decryption, and verification.
/// </summary>
/// <remarks>
/// Implementations must ensure that they dispose of any resources correctly, especially cryptographic ones.
/// </remarks>
public interface ITokenEncrypter : IDisposable
{
    /// <summary>
    /// Encrypts the provided token into a string representation.
    /// </summary>
    /// <param name="token">The token to encrypt.</param>
    /// <returns>The encrypted token string.</returns>
    string Encrypt(IToken token);

    /// <summary>
    /// Decrypts an encrypted token string back to its original form.
    /// </summary>
    /// <param name="encryptedToken">The encrypted token string.</param>
    /// <returns>The original token object.</returns>
    IToken Decrypt(string encryptedToken);

    /// <summary>
    /// Validates whether an encrypted token string is genuine and has not been tampered with.
    /// </summary>
    /// <param name="encryptedToken">The encrypted token string.</param>
    /// <returns>True if the token is valid, otherwise false.</returns>
    bool Verify(string encryptedToken);
}

/// <summary>
/// General-purpose token encrypter that uses an underlying encrypter for its operations.
/// </summary>
public class TokenEncrypter : ITokenEncrypter
{
    private IEncrypter Encrypter { get; }

    /// <summary>
    /// Constructs an instance with a specific underlying encrypter.
    /// </summary>
    /// <param name="encrypter">The encrypter to use.</param>
    public TokenEncrypter(IEncrypter encrypter)
    {
        Encrypter = encrypter ?? throw new ArgumentNullException(nameof(encrypter));
    }

    /// <summary>
    /// Disposes of the encrypter and any associated resources.
    /// </summary>
    public virtual void Dispose()
    {
        Encrypter.Dispose();
    }

    /// <inheritdoc/>
    public virtual string Encrypt(IToken token)
    {
        var json = JsonSerializerSingleton.Serialize(token);
        var bytes = Encoding.UTF8.GetBytes(json);
        var encryptedBytes = Encrypter.Encrypt(bytes);
        var base64Encoded = Convert.ToBase64String(encryptedBytes);

        return WebUtility.UrlEncode(base64Encoded);
    }

    /// <inheritdoc/>
    public virtual IToken Decrypt(string encryptedToken)
    {
        // URL decode the encrypted token
        var base64Decoded = WebUtility.UrlDecode(encryptedToken);
        var bytes = Convert.FromBase64String(base64Decoded);
        var json = Encoding.UTF8.GetString(Encrypter.Decrypt(bytes));

        return JsonSerializerSingleton.Deserialize<Token>(json) ?? throw new InvalidDataException("Failed to deserialize decrypted data.");
    }

    /// <inheritdoc/>
    public virtual bool Verify(string encryptedToken)
    {
        try
        {
            var base64Decoded = WebUtility.UrlDecode(encryptedToken);
            return Encrypter.Verify(Convert.FromBase64String(base64Decoded));
        }
        catch
        {
            return false;
        }
    }

}
