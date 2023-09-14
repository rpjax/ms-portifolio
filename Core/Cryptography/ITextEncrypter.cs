namespace ModularSystem.Core.Cryptography;

/// <summary>
/// Defines the contract for cryptographic operations on textual data.
/// </summary>
/// <remarks>
/// Implementations of this interface should provide mechanisms to ensure confidentiality of textual information
/// through encryption and decryption. This is particularly useful for protecting sensitive strings without
/// having to convert them to byte arrays first. As this interface extends <see cref="IDisposable"/>, 
/// implementations should properly manage and release any resources they acquire.
/// </remarks>
public interface ITextEncrypter : IDisposable
{
    /// <summary>
    /// Encrypts the provided text to ensure its confidentiality.
    /// </summary>
    /// <param name="data">The raw text data to be encrypted.</param>
    /// <returns>A string representation (typically Base64 encoded) of the encrypted version of the provided text.</returns>
    string Encrypt(string data);

    /// <summary>
    /// Decrypts the encrypted text to retrieve its original content.
    /// </summary>
    /// <param name="encryptedData">The encrypted text that needs to be decrypted.</param>
    /// <returns>The original content of the text, after decryption.</returns>
    string Decrypt(string encryptedData);

    /// <summary>
    /// Verifies the integrity or validity of the encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted text whose integrity or validity needs to be verified.</param>
    /// <returns><c>true</c> if the encrypted data is valid or retains its integrity; otherwise, <c>false</c>.</returns>
    bool Verify(string encryptedData);
}
