using ModularSystem.Web;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace ModularSystem.Core.Cryptography;

/// <summary>
/// Provides a base implementation for cryptographic encryption and decryption operations.
/// </summary>
/// <remarks>
/// Derived classes should implement the specific encryption and decryption logic as well as any <br/>
/// required verification mechanisms. This abstract class ensures that all encrypters follow the <br/>
/// <see cref="IEncrypter"/> contract, promoting consistency across different encryption strategies. <br/>
/// As this class implements <see cref="IDisposable"/>, derived classes should also ensure that they <br/>
/// properly handle any resources that need cleanup.
/// </remarks>
public abstract class Encrypter : IEncrypter
{
    /// <summary>
    /// Generates a cryptographically strong random key.
    /// </summary>
    /// <param name="length">The length of the random key in bytes.</param>
    /// <returns>A random key of the specified length.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the length is less than zero.</exception>
    public static byte[] RandomKey(int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");
        }

        return RandomNumberGenerator.GetBytes(length);
    }

    /// <summary>
    /// Encrypts the provided raw data to ensure its confidentiality.
    /// </summary>
    /// <param name="data">The raw data to be encrypted.</param>
    /// <returns>The encrypted version of the provided data.</returns>
    public abstract byte[] Encrypt(byte[] data);

    /// <summary>
    /// Decrypts the encrypted data to retrieve its original content.
    /// </summary>
    /// <param name="encryptedData">The encrypted data that needs to be decrypted.</param>
    /// <returns>The original content of the data, after decryption.</returns>
    public abstract byte[] Decrypt(byte[] encryptedData);

    /// <summary>
    /// Verifies the integrity or authenticity of the specified data.
    /// </summary>
    /// <param name="data">The data whose authenticity or integrity needs to be verified.</param>
    /// <returns><c>true</c> if the data is verified as authentic or intact; otherwise, <c>false</c>.</returns>
    public abstract bool Verify(byte[] data);

    /// <summary>
    /// Releases any resources held by this instance. Derived classes should override this method 
    /// to release any resources they manage.
    /// </summary>
    public virtual void Dispose()
    {
        // Implementation for disposal, if any, should go here.
    }
}

/// <summary>
/// Provides functionalities to encrypt and decrypt textual data using an underlying byte-based encryption scheme.
/// </summary>
/// <remarks>
/// The <see cref="WebTextEncrypter"/> class uses an instance of the <see cref="IEncrypter"/> to perform the actual encryption and decryption
/// of byte arrays. It converts text to byte arrays using UTF-8 encoding before encryption, and decodes them back to text after decryption.
/// Encrypted byte data is then converted to a Base64 encoded string for easier handling and storage.
/// </remarks>
public class WebTextEncrypter : ITextEncrypter
{
    private IEncrypter Encrypter { get; }
    private Encoding Encoding { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebTextEncrypter"/> class with a specified encrypter.
    /// </summary>
    /// <param name="encrypter">The underlying byte-based encrypter.</param>
    /// <param name="encoding">The encoding to be used.</param>
    public WebTextEncrypter(IEncrypter encrypter, Encoding? encoding = null)
    {
        Encrypter = encrypter;
        Encoding = encoding ?? Encoding.UTF8;
    }

    /// <summary>
    /// Disposes the underlying encrypter to ensure any acquired resources are released.
    /// </summary>
    public virtual void Dispose()
    {
        Encrypter.Dispose();
    }

    /// <summary>
    /// Encrypts the provided text and returns its encrypted representation as a Base64 encoded string.
    /// </summary>
    /// <param name="data">The raw text data to be encrypted.</param>
    /// <returns>A Base64 encoded string of the encrypted text.</returns>
    public virtual string Encrypt(string data)
    {
        var bytes = Encoding.GetBytes(data);
        var encryptedBytes = Encrypter.Encrypt(bytes);

        return WebHelper.ToUrlBase64(encryptedBytes);
    }

    /// <summary>
    /// Decrypts a Base64 encoded encrypted text to retrieve its original content.
    /// </summary>
    /// <param name="encryptedData">The Base64 encoded encrypted text that needs to be decrypted.</param>
    /// <returns>The original content of the text, after decryption.</returns>
    public virtual string Decrypt(string encryptedData)
    {
        var encryptedBytes = WebHelper.FromUrlBase64(encryptedData);
        var bytes = Encrypter.Decrypt(encryptedBytes);

        return Encoding.GetString(bytes);
    }

    /// <inheritdoc/>
    public virtual bool Verify(string encryptedData)
    {
        try
        {
            var encryptedBytes = WebHelper.FromUrlBase64(encryptedData);

            return Encrypter.Verify(encryptedBytes);
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Default implementation of the <see cref="ISaltGenerator"/> interface.
/// </summary>
/// <remarks>
/// This implementation uses a predefined set of characters to generate a diverse and complex salt string.
/// The generated salt can be used to strengthen passwords or other data in cryptographic operations.
/// </remarks>
public class SaltGenerator : ISaltGenerator
{
    /// <summary>
    /// Set of characters used in the generation of salt strings. This set encompasses uppercase letters, 
    /// lowercase letters, digits, and a variety of special characters to produce a richer salt.
    /// </summary>
    private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-+=!@#$%^&*(){}[]|:;<>,.?~";

    public virtual void Dispose()
    {

    }

    /// <inheritdoc/>
    public virtual string Generate(int length)
    {
        var saltChars = new char[length];
        var byteSalt = RandomNumberGenerator.GetBytes(length);

        for (int i = 0; i < length; i++)
        {
            saltChars[i] = CHARS[byteSalt[i] % CHARS.Length];
        }

        return new string(saltChars);
    }

    /// <inheritdoc/>
    public virtual byte[] GenerateBytes(int length)
    {
        return RandomNumberGenerator.GetBytes(length);
    }
}


// BIG TODO
public class PrimeGenerator
{
    readonly int bitLength;

    public PrimeGenerator(int bitLength)
    {
        this.bitLength = bitLength;
        bytes = new byte[bitLength];
    }

    byte[] bytes;

    public BigInteger Compute()
    {
        // It's not random enough
        // NOT ready to be used
        bytes = RandomBytes(bitLength);
        for (var i = 0; i < 10; i++)
        {
            var key = RandomBytes(bitLength);
            bytes = ShuffleByteArray(bytes, key);
        }
        var number = new BigInteger(bytes);
        if (number.Sign == -1)
        {
            number = BigInteger.Negate(number);
        }
        while (!IsPrime(number))
        {
            number--;
            break;
        }
        return number;
    }

    public void Test()
    {
        var random = new Random();
        var data = new List<long>();
        var rounds = 10000000;
        var g = 4;
        for (int i = 0; i < 256; i++)
        {
            data.Add(0);
        }
        for (var i = 0; i < rounds; i++)
        {
            var a = (int)BigInteger.ModPow(g, i, 251);
            data[a] = data[a] + 1;
        }
        for (int i = 0; i < data.Count; i++)
        {
            var num = data[i];
            var percentage = decimal.Divide(num, rounds);
            Console.WriteLine($"{i} - {percentage * 100}");
        }
        return;

        for (int i = 0; i < rounds; i++)
        {
            var byteA = Convert.ToByte(random.Next(0, 255));
            var byteB = Convert.ToByte(random.Next(0, 255));
            var c = RandomizeByte(byteA, byteB);
            data[c] = data[c] + 1;
        }
        for (int i = 0; i < data.Count; i++)
        {
            var num = data[i];
            var percentage = decimal.Divide(num, rounds);
            Console.WriteLine($"{i} - {percentage * 100}");
        }
    }

    bool IsPrime(BigInteger num)
    {
        // TODO
        return false;
    }

    byte[] RandomBytes(int bitLength)
    {
        var bytes = new byte[bitLength];
        // Avaliate the importance of the seed generation
        //var seed = BitConverter.ToInt32(SHA256.HashData(BitConverter.GetBytes(DateTime.Now.Millisecond)));
        var random = new Random();
        for (int i = 0; i < bitLength; i++)
        {
            var b = Convert.ToByte(random.Next(0, 255));
            bytes[i] = b;
        }
        return bytes;
    }

    byte[] ShuffleByteArray(byte[] a, byte[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            var _byte = a[i];
            var key = b[i];
            a[i] = RandomizeByte(_byte, key);
        }
        return a;
    }

    byte RandomizeByte(byte a, byte b)
    {
        while (a == 0)
        {
            a = RandomBytes(1)[0];
        }
        if (b == 0)
        {
            b = Convert.ToByte(a / 2);
        }
        var c = (a * b) % 256;
        return Convert.ToByte(c);
    }

}

public class DiffieHellmanKeyExchange
{
    public long G { get; set; }

    public byte[] ComputeSharedKey()
    {
        return new byte[0];
    }
}