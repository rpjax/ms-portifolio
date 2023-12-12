using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Core.Cryptography;
using ModularSystem.Core.Helpers;
using ModularSystem.Core.Security;

namespace ModularSystem.Web.Authentication;

/// <summary>
/// Provides authentication services using AES encrypted tokens. <br/>
/// This implementation supports token encryption and salt generation for enhanced security.
/// </summary>
public class AesAuthenticationProvider : IAuthenticationProvider
{
    /// <summary>
    /// The default token lifetime, set to 24 hours.
    /// </summary>
    public static TimeSpan DefaultTokenLifetime { get; } = TimeSpan.FromHours(24);

    /// <summary>
    /// Duration after which the token expires.
    /// </summary>
    public TimeSpan TokenLifetime { get; set; }

    /// <summary>
    /// Gets information about the file where the encryption key is stored.
    /// </summary>
    protected FileInfo? EncryptionKeyFileInfo { get; }

    /// <summary>
    /// Cache for the encryption key used by the encrypter.
    /// </summary>
    protected byte[]? EncryptionKeyCache { get; set; } = null;

    /// <summary>
    /// Object used to synchronize access to the encryption key cache.
    /// </summary>
    protected object EncryptionKeyLock { get; } = new object();

    /// <summary>
    /// Responsible for encrypting and decrypting tokens.
    /// </summary>
    protected ITokenEncrypter TokenEncrypter { get; }

    /// <summary>
    /// Generates salts for token creation.
    /// </summary>
    protected ISaltGenerator SaltGenerator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AesAuthenticationProvider"/> class.
    /// </summary>
    /// <param name="options">Parameters for token life cycle and other configurations.</param>
    public AesAuthenticationProvider(Options? options = null)
    {
        options ??= new Options();

        TokenLifetime = options.TokenLifetime;
        EncryptionKeyFileInfo = options?.KeyFileInfo;
        TokenEncrypter = CreateTokenEncrypter();
        SaltGenerator = new SaltGenerator();
    }

    /// <summary>
    /// Extracts and decrypts the bearer token from the provided HTTP context.
    /// </summary>
    public virtual IToken? GetToken(HttpContext httpContext)
    {
        var rawToken = httpContext.GetBearerToken();

        if (rawToken == null)
        {
            return null;
        }

        if (!TokenEncrypter.Verify(rawToken))
        {
            throw new AppException("Invalid or unrecognized credentials provided. Authentication failed.", ExceptionCode.CredentialsInvalid);
        }

        return TokenEncrypter.Decrypt(rawToken);
    }

    /// <summary>
    /// Creates a token with prefixed and suffixed salts from the provided identity.
    /// </summary>
    public virtual IToken GetToken(IIdentity identity)
    {
        if (identity is not Identity)
        {
            throw new ArgumentException("Only 'ModularSystem.Core.Security.Identity' is supported by this Auth provider.");
        }

        var random = new Random();
        var prefixSaltSize = random.Next(150, 500);
        var sufixSaltSize = random.Next(150, 500);
        var now = TimeProvider.Now();
        var lifetime = TokenLifetime;
        var payload = JsonSerializerSingleton.Serialize((Identity)identity);

        return new Token()
        {
            PrefixSalt = SaltGenerator.Generate(prefixSaltSize),
            Payload = payload,
            SuffixSalt = SaltGenerator.Generate(sufixSaltSize),
            CreatedAt = now,
            ExpiresAt = now.Add(lifetime),
        };
    }

    /// <summary>
    /// Deserializes the token payload into an identity instance.
    /// </summary>
    public virtual IIdentity? GetIdentity(IToken token)
    {
        try
        {
            if (token.Payload == null)
            {
                throw new AppException("Invalid or unrecognized credentials provided. Authentication failed.", ExceptionCode.CredentialsInvalid);
            }

            return JsonSerializerSingleton.Deserialize<Identity>(token.Payload);
        }
        catch (Exception e)
        {
            throw new AppException("Failed to deserialize the token's payload.", ExceptionCode.Internal, e)
                .AddData(token);
        }
    }

    /// <summary>
    /// Retrieves the current token encrypter used for encryption and decryption of authentication tokens.
    /// </summary>
    /// <returns>The <see cref="ITokenEncrypter"/> instance responsible for token encryption and decryption.</returns>
    public ITokenEncrypter GetTokenEncrypter()
    {
        return TokenEncrypter;
    }

    /// <summary>
    /// Provides the encryption key, initializing it from cache or storage.
    /// </summary>
    byte[] EncryptionKey()
    {
        if (EncryptionKeyCache == null)
        {
            lock (EncryptionKeyLock)
            {
                InitEncryptionKeyCache();
            }
        }

        return EncryptionKeyCache!;
    }

    /// <summary>
    /// Initializes the encryption key cache either from storage or by generating a new one if none exists.
    /// </summary>
    void InitEncryptionKeyCache()
    {
        var fileInfo = EncryptionKeyFileInfo ?? LocalStorage.GetFileInfo("websession_encryption_key.json");
        var storage = new JsonStorage<KeyFile>(fileInfo);
        var file = storage.Read();

        if (file == null)
        {
            file = new KeyFile();
        }
        if (file.Bytes == null || file.Bytes.IsEmpty())
        {
            file.Bytes = AesEncrypter.RandomKey(AesKeySize.bits256);
            storage.Write(file);
        }

        EncryptionKeyCache = file.Bytes;
    }

    /// <summary>
    /// Returns the token encrypter, which is initialized with the encryption key.
    /// </summary>
    ITokenEncrypter CreateTokenEncrypter()
    {
        var encrypter = new AesEncrypter(EncryptionKey(), new byte[16]);
        return new TokenEncrypter(encrypter);
    }

    /// <summary>
    /// Configuration parameters for the <see cref="AesAuthenticationProvider"/>.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Specifies the lifetime of tokens generated by the provider.
        /// </summary>
        public TimeSpan TokenLifetime { get; set; } = DefaultTokenLifetime;

        /// <summary>
        /// Gets information about the file where the encryption key is stored.
        /// </summary>
        public FileInfo? KeyFileInfo { get; set; } = null;
    }

    /// <summary>
    /// Represents the stored encryption key file.
    /// </summary>
    internal class KeyFile
    {
        /// <summary>
        /// Bytes of the stored encryption key.
        /// </summary>
        public byte[]? Bytes { get; set; } = new byte[0];
    }
}