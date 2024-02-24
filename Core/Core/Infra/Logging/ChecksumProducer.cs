using System.Security.Cryptography;
using System.Text;

namespace ModularSystem.Core.Logging;

public interface IChecksumProducer
{
    /// <summary>
    /// Computes a checksum hash that is unique to the input. 
    /// </summary>
    /// <returns></returns>
    byte[] ComputeChecksum();
}

[Obsolete("UNDER DEVELOPMENT, AND REALLY OLD STUFF")]
public abstract class ChecksumProducer : IChecksumProducer
{
    public enum Algorithm
    {
        SHA256,
        SHA512
    }

    public ChecksumProducer(Algorithm algorithm = Algorithm.SHA512)
    {
      
    }

    /// <summary>
    /// Computes a checksum hash that is unique to the input. 
    /// </summary>
    /// <returns></returns>
    public byte[] ComputeChecksum()
    {
        using (var sha = SHA512.Create())
        {
            return sha.ComputeHash(Encoding.UTF8.GetBytes(GetChecksumState()));
        }
    }

    /// <summary>
    /// computes the checksum and returns a base64 representation of the byte array.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Convert.ToBase64String(ComputeChecksum());
    }

    protected abstract string GetChecksumState();

}