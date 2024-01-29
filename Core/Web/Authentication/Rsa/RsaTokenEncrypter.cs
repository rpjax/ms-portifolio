using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ModularSystem.Web.Authentication;

//public sealed class RsaTokenEncrypter
//{
//    readonly RSACryptoServiceProvider rsa;

//    public RsaTokenEncrypter(RSAParameters parameters)
//    {
//        rsa = new RSACryptoServiceProvider();
//        rsa.ImportParameters(parameters);
//    }

//    public void Dispose()
//    {
//        rsa.Dispose();
//    }

//    public string Encrypt(IToken token)
//    {
//        var json = JsonSerializer.Serialize(token);
//        var bytes = Encoding.UTF8.GetBytes(json);
//        var encrypted = rsa.Encrypt(bytes, false);
//        return Convert.ToBase64String(encrypted);
//    }

//    public IToken Decrypt(string encryptedToken)
//    {
//        var bytes = Convert.FromBase64String(encryptedToken);
//        var decrypted = rsa.Decrypt(bytes, false);
//        var json = Encoding.UTF8.GetString(decrypted);
//        return JsonSerializer.Deserialize<Token>(json) ?? new Token();
//    }

//    public bool Verify(string encryptedToken)
//    {
//        try
//        {
//            var bytes = Convert.FromBase64String(encryptedToken);
//            var decrypted = rsa.Decrypt(bytes, false);
//            return true;
//        }
//        catch (Exception)
//        {
//            return false;
//        }
//    }
//}