using System.Security.Cryptography;

namespace ModularSystem.Core.Cryptography;

/// <summary>
/// Represents different AES encryption key sizes.
/// </summary>
public enum AesKeySize
{
    bits128 = 128,
    bits192 = 192,
    bits256 = 256,
}

/// <summary>
/// Provides AES encryption and decryption functionalities.
/// </summary>
/// <remarks>
/// The <see cref="AesEncrypter"/> class provides methods to encrypt and decrypt data using the Advanced Encryption Standard (AES) algorithm.
/// It allows for the specification of a key and an optional initialization vector (IV). Utility methods are provided to generate random keys and IVs.
/// </remarks>
public class AesEncrypter : Encrypter
{
    /// <summary>
    /// Gets the AES encryption and decryption provider.
    /// </summary>
    private Aes Aes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AesEncrypter"/> class using the provided key.
    /// </summary>
    /// <param name="key">The encryption key.</param>
    public AesEncrypter(byte[] key)
    {
        Aes = Aes.Create();
        Aes.Key = key;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AesEncrypter"/> class using the provided key and IV.
    /// </summary>
    /// <param name="key">The encryption key.</param>
    /// <param name="iv">The initialization vector.</param>
    public AesEncrypter(byte[] key, byte[] iv)
    {
        Aes = Aes.Create();
        Aes.Key = key;
        Aes.IV = iv;
    }

    /// <summary>
    /// Creates an instance of the <see cref="AesEncrypter"/> with a random key and IV of the specified size.
    /// </summary>
    /// <param name="keySize">The size of the AES key. Defaults to 128 bits.</param>
    /// <returns>An instance of <see cref="AesEncrypter"/>.</returns>
    public static AesEncrypter Random(AesKeySize keySize = AesKeySize.bits128)
    {
        return new AesEncrypter(CryptographyUtils.GenerateAesKey(keySize), CryptographyUtils.GenerateAesIv());
    }

    /// <summary>
    /// Generates a random AES key of the specified size.
    /// </summary>
    /// <param name="keySize">The size of the AES key. Defaults to 128 bits.</param>
    /// <returns>A random AES key.</returns>
    public static byte[] RandomKey(AesKeySize keySize = AesKeySize.bits128)
    {
        return CryptographyUtils.GenerateAesKey(keySize);
    }

    /// <summary>
    /// Disposes the underlying AES provider and releases any acquired resources.
    /// </summary>
    public override void Dispose()
    {
        Aes.Dispose();
        base.Dispose();
    }

    /// <inheritdoc/>
    public override byte[] Encrypt(byte[] data)
    {
        byte[] encryptedData;
        ValidateByteArray(data);

        using var encryptor = Aes.CreateEncryptor();
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.Flush();

        encryptedData = memoryStream.ToArray();

        return encryptedData;
    }

    /// <inheritdoc/>
    public override byte[] Decrypt(byte[] encryptedData)
    {
        var decryptor = Aes.CreateDecryptor();
        ValidateByteArray(encryptedData);

        using var memoryStream = new MemoryStream(encryptedData);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cryptoStream);
        using var buffer = new MemoryStream();

        reader.BaseStream.CopyTo(buffer);
        return buffer.ToArray();
    }

    /// <inheritdoc/>
    public override bool Verify(byte[] data)
    {
        try
        {
            Decrypt(data);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Validates the provided byte array to ensure it is suitable for encryption or decryption.
    /// </summary>
    /// <param name="data">The byte array to be validated.</param>
    /// <exception cref="AppException">Thrown when the byte array has a length of zero.</exception>
    void ValidateByteArray(byte[] data)
    {
        if (data.Length == 0)
        {
            throw new AppException("Aes encryptor cannot work with a byte array of zero length.");
        }
    }
}
