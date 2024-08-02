using Aidan.Core.Cryptography;
using Aidan.Web.AccessManagement.Tokens;
using System.Text;
using System.Text.Json;

namespace Aidan.Web.Authentication;

/// <summary>
/// A token encrypter that utilizes the AES encryption standard.
/// </summary>
/// <remarks>
/// It is important to handle instances of this class with care to ensure that cryptographic resources are disposed of correctly.
/// </remarks>
public sealed class AesTokenEncrypter 
{
    private AesEncrypter Encrypter { get; }

    public AesTokenEncrypter(AesEncrypter encrypter)
    {
        Encrypter = encrypter;
    }

    public void Dispose()
    {
        Encrypter.Dispose();
    }

    public string Encrypt(WebToken token)
    {
        var json = JsonSerializer.Serialize(token);
        var bytes = Encoding.UTF8.GetBytes(json);
        var encryptedBytes = Encrypter.Encrypt(bytes);
        return Convert.ToBase64String(encryptedBytes);
    }

    public WebToken Decrypt(string encryptedToken)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedToken);
        var decryptedBytes = Encrypter.Decrypt(encryptedBytes);
        var json = Encoding.UTF8.GetString(decryptedBytes);
        return JsonSerializer.Deserialize<WebToken>(json) ?? throw new InvalidDataException();
    }

    public bool Verify(string encryptedToken)
    {
        try
        {
            return Encrypter.Verify(Convert.FromBase64String(encryptedToken));
        }
        catch (Exception)
        {
            return false;
        }
    }
}
