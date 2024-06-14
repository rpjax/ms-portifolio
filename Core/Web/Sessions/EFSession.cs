//using ModularSystem.Core;
//using ModularSystem.Core.Cryptography;
//using ModularSystem.Core.Security;
//using ModularSystem.EntityFramework;
//using System.Message.Json;

//namespace ModularSystem.Web;

///// <summary>
///// Implementation of <see cref="ISession"/> that implements <see cref="EFModel"/> 
///// </summary>
//public class EFSession : EFModel, ISession
//{
//    public bool IsEncrypted { get; set; }
//    public byte[] EncryptionKey { get => _encryptionKey; set => SetEncryptionKey(value); }
//    public DateTime LastUsedAt { get; set; } = DateTime.MinValue;
//    public DateTime EncryptionKeyLastUpdatedAt { get; set; } = DateTime.MinValue;
//    public DateTime ExpiresAt { get; set; } = DateTime.MinValue;
//    public string? SerializedIdentity { get; set; } = null;
//    public string? IdentityIdentifier { get; set; } = null;

//    protected byte[] _encryptionKey;
//    protected ISessionManager _manager;
//    protected Identity? _identity = null;

//    public EFSession()
//    {
//        _encryptionKey = new byte[0];
//        _manager = DependencyContainer.GetInterface<ISessionManager>();
//    }

//    public EFSession(TimeSpan lifetime, Identity identity) : this()
//    {
//        ExpiresAt = CreatedAt.Add(lifetime);
//        _identity = identity;
//    }

//    public bool IsExpired()
//    {
//        return TimeProvider.Now() > ExpiresAt;
//    }

//    public TimeSpan GetLifetime()
//    {
//        return ExpiresAt - TimeProvider.Now();
//    }

//    public IIdentity GetIdentity()
//    {
//        if (_identity != null)
//        {
//            return _identity;
//        }
//        if (SerializedIdentity == null)
//        {
//            return new Identity("");
//        }

//        _identity = JsonSerializer.Deserialize<Identity>(SerializedIdentity);

//        if (_identity == null)
//        {
//            throw new InvalidOperationException("Could not deserialize the Identity.");
//        }

//        return _identity;
//    }

//    public IEncrypter GetEncrypter()
//    {
//        return _manager.GetDataEncrypter(EncryptionKey);
//    }

//    public EFSession SetIdentity(Identity identity)
//    {
//        _identity = identity;
//        SerializedIdentity = JsonSerializer.Serialize(identity);
//        IdentityIdentifier = identity.UniqueIdentifier;
//        return this;
//    }

//    public EFSession SetEncryptionKey(byte[] bytes)
//    {
//        _encryptionKey = bytes;
//        EncryptionKeyLastUpdatedAt = TimeProvider.Now();
//        return this;
//    }

//    public byte[] Encrypt(byte[] data)
//    {
//        return GetEncrypter().Encrypt(data);
//    }

//    public byte[] Decrypt(byte[] encryptedData)
//    {
//        return GetEncrypter().Decrypt(encryptedData);
//    }

//    public void ExtendLifeTime(TimeSpan ticks)
//    {
//        ExpiresAt = ExpiresAt.Add(ticks);
//    }
//}