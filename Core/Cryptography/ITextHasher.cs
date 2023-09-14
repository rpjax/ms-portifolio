namespace ModularSystem.Core.Cryptography;

/// <summary>
/// Defines the contract for hashing textual data.
/// </summary>
/// <remarks>
/// Implementations of this interface should provide mechanisms to hash textual data. This ensures 
/// data integrity and can be used for various security and data verification processes. 
/// Since the interface implements <see cref="IDisposable"/>, any acquired resources should be properly managed and released.
/// </remarks>
public interface ITextHasher : IDisposable
{
    /// <summary>
    /// Computes the hash value of a given text string.
    /// </summary>
    /// <param name="text">The text string to hash.</param>
    /// <returns>An array of bytes representing the hash value.</returns>
    byte[] GetHash(string text);

    /// <summary>
    /// Computes the hash value of a given text string and returns its base64 encoded representation.
    /// </summary>
    /// <param name="text">The text string to hash.</param>
    /// <returns>A base64 encoded string of the hashed value.</returns>
    string GetHashString(string text);
}