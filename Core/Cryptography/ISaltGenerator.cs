namespace Aidan.Core.Cryptography;

/// <summary>
/// Represents the contract for salt generation in cryptographic operations.
/// </summary>
/// <remarks>
/// Salting is a technique used to augment passwords or data in order to defend against dictionary and rainbow table attacks.
/// Implementations of this interface should provide mechanisms to generate salt in both string and byte array formats. 
/// As this interface extends <see cref="IDisposable"/>, implementations should properly manage and release any resources they acquire.
/// </remarks>
public interface ISaltGenerator : IDisposable
{
    /// <summary>
    /// Produces a salt string of a designated length.
    /// </summary>
    /// <param name="length">The desired length of the generated salt string.</param>
    /// <returns>A salt string of the specified length.</returns>
    string Generate(int length);

    /// <summary>
    /// Produces a salt byte array of a designated length.
    /// </summary>
    /// <param name="length">The desired length of the generated salt byte array.</param>
    /// <returns>A salt byte array of the specified length.</returns>
    byte[] GenerateBytes(int length);
}
