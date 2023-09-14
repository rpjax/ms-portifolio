using System.Security.Cryptography;

namespace ModularSystem.Core.Logging;

public interface IChecksum
{
    /// <summary>
    /// Computes a checksum hash that is unique to the input. 
    /// </summary>
    /// <returns></returns>
    byte[] Compute();
}

public class Checksum : IChecksum
{
    byte[] data;

    public Checksum(byte[] data)
    {
        this.data = data;
    }

    public static bool operator ==(Checksum self, Checksum other)
    {
        if (self.data.Length != other.data.Length)
        {
            return false;
        }

        return self.Compute().SequenceEqual(other.Compute());
    }

    public static bool operator !=(Checksum self, Checksum other)
    {
        return !(self == other);
    }

    /// <summary>
    /// Sets the data to check. 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Checksum SetData(byte[] data)
    {
        this.data = data;
        return this;
    }

    /// <summary>
    /// Computes a checksum hash that is unique to the input. 
    /// </summary>
    /// <returns></returns>
    public byte[] Compute()
    {
        using (var sha = SHA512.Create())
        {
            return sha.ComputeHash(data);
        }
    }

    public bool Check(byte[] checksum)
    {
        return Compute().SequenceEqual(checksum);
    }

    /// <summary>
    /// computes the checksum and returns a base64 representation of the byte array.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Convert.ToBase64String(Compute());
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}