using ModularSystem.Core;
using ModularSystem.Core.AccessManagement;

namespace ModularSystem.Web;

// session is a memory space bound to an ID that saves some state and data
public interface ISession : IEntity
{
    bool IsEncrypted { get; set; }
    byte[] EncryptionKey { get; set; }

    DateTime ExpiresAt { get; set; }
    DateTime LastUsedAt { get; set; }
    DateTime EncryptionKeyLastUpdatedAt { get; set; }

    bool IsExpired();
    TimeSpan GetLifetime();
    IIdentity GetIdentity();

    byte[] Encrypt(byte[] data);
    byte[] Decrypt(byte[] encryptedData);
}