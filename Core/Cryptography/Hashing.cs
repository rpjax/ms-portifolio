using ModularSystem.Web;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace ModularSystem.Core.Cryptography;

//*
// Hashing
//*

/// <summary>
/// Represents a container for salt data used in cryptographic hashing operations.
/// </summary>
/// <remarks>
/// Salts are random values that are combined with passwords or other data 
/// before being passed through a hash function. Using salts can greatly increase the security of hashed values 
/// as they ensure that the same input data will produce different hash outputs if different salts are used.
/// </remarks>
public class HashSalt
{
    /// <summary>
    /// Gets or sets the byte representation of the salt.
    /// </summary>
    /// <value>The byte array containing the salt data.</value>
    public byte[] Bytes { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HashSalt"/> class.
    /// </summary>
    /// <param name="bytes">An optional byte array representing the salt. If not provided, an empty byte array will be used.</param>
    [JsonConstructor]
    public HashSalt(byte[]? bytes = null)
    {
        Bytes = bytes ?? new byte[0];
    }
}

/// <summary>
/// Provides context for hashing operations by maintaining a collection of salts.
/// </summary>
/// <remarks>
/// The <see cref="HashContext"/> serves as a repository for multiple salts, allowing for diverse cryptographic operations. 
/// This can be particularly useful for scenarios where different salts are used for different 
/// pieces of data or for rotating salts periodically to enhance security.
/// </remarks>
public class HashContext
{
    /// <summary>
    /// Gets or sets the list of salts used in hashing operations.
    /// </summary>
    /// <value>The list of <see cref="HashSalt"/> instances.</value>
    public List<HashSalt> Salts { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HashContext"/> class.
    /// </summary>
    /// <param name="salts">An optional collection of <see cref="HashSalt"/> objects. If not provided, an empty list will be used.</param>
    [JsonConstructor]
    public HashContext(IEnumerable<HashSalt>? salts = null)
    {
        Salts = salts?.ToList() ?? new();
    }
}

/// <summary>
/// Represents the default implementation of the <see cref="ITextHasher"/> interface.
/// </summary>
/// <remarks>
/// Utilizes the provided <see cref="HashAlgorithm"/> to perform hashing operations on text data.
/// </remarks>
public class TextHasher : ITextHasher
{
    /// <summary>
    /// Gets the underlying hash algorithm used to compute hash values.
    /// </summary>
    private HashAlgorithm HashAlgorithm { get; }

    /// <summary>
    /// Constructs a new instance of <see cref="TextHasher"/> with a specified hash algorithm.
    /// </summary>
    /// <param name="hashAlgorithm">The specific hash algorithm to be used.</param>
    public TextHasher(HashAlgorithm hashAlgorithm)
    {
        HashAlgorithm = hashAlgorithm;
    }

    /// <summary>
    /// Releases the resources used by the <see cref="HashAlgorithm"/>.
    /// </summary>
    public virtual void Dispose()
    {
        HashAlgorithm.Dispose();
    }

    /// <inheritdoc/>
    public virtual byte[] GetHash(string text)
    {
        return HashAlgorithm.ComputeHash(text.ToMemoryStream());
    }

    /// <inheritdoc/>
    public virtual string GetHashString(string text)
    {
        return WebHelper.ToUrlBase64(GetHash(text));
    }
}
