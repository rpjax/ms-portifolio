using ModularSystem.Core.Extensions;
using System.Security.Cryptography;
using System.Text;

namespace ModularSystem.Core.Cryptography;

public class CryptographyUtils
{
    public static RSAParameters GenerateRandomRsaParams(int bits = 2048)
    {
        var service = new RSACryptoServiceProvider(bits);
        return service.ExportParameters(true);
    }

    public static byte[] GenerateAesKey(AesKeySize size = AesKeySize.bits128)
    {
        var aes = Aes.Create();
        byte[] key;
        aes.KeySize = (int)size;
        aes.GenerateKey();
        key = aes.Key;
        aes.Dispose();
        return key;
    }

    public static byte[] GenerateAesIv()
    {
        var aes = Aes.Create();
        byte[] iv;
        aes.GenerateIV();
        iv = aes.IV;
        aes.Dispose();
        return iv;
    }

    public static string Hash256Base64(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(bytes);
        return Encoding.UTF8.GetString(hash).EncodeBase64();
    }
}
