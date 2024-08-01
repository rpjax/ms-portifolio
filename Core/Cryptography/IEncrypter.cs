namespace ModularSystem.Core.Cryptography;

/// <summary>
/// Defines a contract for cryptographic encryption and decryption operations.
/// </summary>
/// <remarks>
/// Implementations of this interface should ensure data confidentiality through encryption 
/// and allow the retrieval of the original data through decryption. Additionally, they should provide 
/// a mechanism for verifying the integrity or authenticity of the data. As this interface extends 
/// <see cref="IDisposable"/>, implementations should properly manage and release any resources they acquire.
/// </remarks>
public interface IEncrypter : IDisposable
{
    /// <summary>
    /// Encrypts the specified data, ensuring its confidentiality.
    /// </summary>
    /// <param name="data">The raw data to be encrypted.</param>
    /// <returns>The encrypted form of the provided data.</returns>
    byte[] Encrypt(byte[] data);

    /// <summary>
    /// Decrypts the specified encrypted data, retrieving the original content.
    /// </summary>
    /// <param name="encryptedData">The data that has been encrypted and needs decryption.</param>
    /// <returns>The decrypted form of the provided data, revealing its original content.</returns>
    byte[] Decrypt(byte[] encryptedData);

    /// <summary>
    /// Verifies the integrity or authenticity of the specified data.
    /// </summary>
    /// <param name="encryptedData">The data to be verified.</param>
    /// <returns><c>true</c> if the data is verified to be authentic or intact; otherwise, <c>false</c>.</returns>
    bool Verify(byte[] encryptedData);
}
