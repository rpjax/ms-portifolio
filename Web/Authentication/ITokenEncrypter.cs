using ModularSystem.Core;
using ModularSystem.Core.Cryptography;

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
    string Encrypt(Token token);

    /// <summary>
    /// Decrypts an encrypted token string back to its original form.
    /// </summary>
    /// <param name="encryptedToken">The encrypted token string.</param>
    /// <returns>The original token object.</returns>
    Token Decrypt(string encryptedToken);

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
    private WebEncodedTextEncrypter Encrypter { get; }

    /// <summary>
    /// Constructs an instance with a specific underlying encrypter.
    /// </summary>
    /// <param name="encrypter">The encrypter to use.</param>
    public TokenEncrypter(IEncrypter encrypter)
    {
        if (encrypter == null)
        {
            throw new ArgumentNullException(nameof(encrypter));
        }

        Encrypter = new WebEncodedTextEncrypter(encrypter);
    }

    /// <summary>
    /// Disposes of the encrypter and any associated resources.
    /// </summary>
    public virtual void Dispose()
    {
        Encrypter.Dispose();
    }

    /// <inheritdoc/>
    public virtual string Encrypt(Token token)
    {
        return Encrypter.Encrypt(JsonSerializerSingleton.Serialize(token));
    }

    /// <inheritdoc/>
    public virtual Token Decrypt(string encryptedToken)
    {
        var json = Encrypter.Decrypt(encryptedToken);
        return
            JsonSerializerSingleton.Deserialize<Token>(json)
            ?? throw new InvalidDataException("Failed to deserialize decrypted data.");
    }

    /// <inheritdoc/>
    public virtual bool Verify(string encryptedToken)
    {
        try
        {
            return Encrypter.Verify(encryptedToken);
        }
        catch
        {
            return false;
        }
    }

}
