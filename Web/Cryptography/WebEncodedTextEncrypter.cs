using Aidan.Core.Cryptography;
using System.Text;

namespace Aidan.Web.Cryptography;

/// <summary>
/// Provides functionalities to encrypt and decrypt textual data using an underlying byte-based encryption scheme.
/// </summary>
/// <remarks>
/// The <see cref="WebEncodedTextEncrypter"/> class uses an instance of the <see cref="IEncrypter"/> to perform the actual encryption and decryption
/// of byte arrays. It converts text to byte arrays using UTF-8 encoding before encryption, and decodes them back to text after decryption.
/// Encrypted byte data is then converted to a Base64 encoded string for easier handling and storage.
/// </remarks>
public class WebEncodedTextEncrypter : ITextEncrypter
{
    private IEncrypter Encrypter { get; }
    private Encoding Encoding { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebEncodedTextEncrypter"/> class with a specified encrypter.
    /// </summary>
    /// <param name="encrypter">The underlying byte-based encrypter.</param>
    /// <param name="encoding">The encoding to be used.</param>
    public WebEncodedTextEncrypter(IEncrypter encrypter, Encoding? encoding = null)
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

        return WebEncodingHelper.ToWebEncodedBase64(encryptedBytes);
    }

    /// <summary>
    /// Decrypts a Base64 encoded encrypted text to retrieve its original content.
    /// </summary>
    /// <param name="encryptedData">The Base64 encoded encrypted text that needs to be decrypted.</param>
    /// <returns>The original content of the text, after decryption.</returns>
    public virtual string Decrypt(string encryptedData)
    {
        var encryptedBytes = WebEncodingHelper.FromWebEncodedBase64(encryptedData);
        var bytes = Encrypter.Decrypt(encryptedBytes);

        return Encoding.GetString(bytes);
    }

    /// <inheritdoc/>
    public virtual bool Verify(string encryptedData)
    {
        try
        {
            return Encrypter.Verify(WebEncodingHelper.FromWebEncodedBase64(encryptedData));
        }
        catch
        {
            return false;
        }
    }
}
