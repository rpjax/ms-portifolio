using ModularSystem.Core.Cryptography;
using System.Text;
using System.Text.Json;

namespace ModularSystem.Web.Authentication;

/// <summary>
/// A token encrypter that utilizes the AES encryption standard.
/// </summary>
/// <remarks>
/// It is important to handle instances of this class with care to ensure that cryptographic resources are disposed of correctly.
/// </remarks>
public sealed class AesTokenEncrypter : ITokenEncrypter
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

    public string Encrypt(IToken token)
    {
        var json = JsonSerializer.Serialize(token);
        var bytes = Encoding.UTF8.GetBytes(json);
        var encryptedBytes = Encrypter.Encrypt(bytes);
        return Convert.ToBase64String(encryptedBytes);
    }

    public IToken Decrypt(string encryptedToken)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedToken);
        var decryptedBytes = Encrypter.Decrypt(encryptedBytes);
        var json = Encoding.UTF8.GetString(decryptedBytes);
        return JsonSerializer.Deserialize<Token>(json) ?? throw new InvalidDataException();
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
