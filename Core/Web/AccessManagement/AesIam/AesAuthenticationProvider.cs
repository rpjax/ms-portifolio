using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Core.Cryptography;
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

    /// <inheritdoc/>
    public virtual async Task<OperationResult<IIdentity>> TryGetIdentityAsync(HttpContext httpContext)
    {
        var tokenResult = TryGetTokenFromContext(httpContext);

        if (tokenResult.IsFailure)
        {
            return new(tokenResult.Errors);
        }

        if(tokenResult.Data == null)
        {
            return new(true);
        }

        var token = tokenResult.Data;

        if (token.Payload == null)
        {
            var message = "Invalid token payload: The provided bearer token lacks the necessary data for identity verification.";
            var error = new Error(message)
                .AddJsonData("Token", token)
                .AddFlags(ErrorFlags.Critical, ErrorFlags.Debug);

            return new(error);
        }

        if (token.IsExpired())
        {
            var message = "The provided credential is expired and can no longer be used.";
            var error = new Error(message)
                .AddJsonData("Token", token)
                .AddFlags(ErrorFlags.Public);

            return new(true, null, error);
        }

        var identity = JsonSerializerSingleton.Deserialize<Identity>(token.Payload);

        if(identity == null)
        {
            var message = "Identity processing error: Unable to extract identity information from the token due to deserialization issues.";
            var error = new Error(message)
                .AddJsonData("Token", token)
                .AddFlags(ErrorFlags.Critical, ErrorFlags.Debug);

            return new(error);
        }

        return new(identity);
    }

    /// <summary>
    /// Creates a token with prefixed and suffixed salts from the provided identity.
    /// </summary>
    public virtual Token CreateToken(Identity identity)
    {
        var random = new Random();
        var now = TimeProvider.Now();

        var prefixSaltSize = random.Next(100, 150);
        var suffixSaltSize = random.Next(100, 150);

        var prefixSalt = SaltGenerator.Generate(prefixSaltSize);
        var payload = JsonSerializerSingleton.Serialize(identity);
        var suffixSalt = SaltGenerator.Generate(suffixSaltSize);
        var expiresAt = now.Add(TokenLifetime);

        return new Token()
        {
            PrefixSalt = prefixSalt,
            Payload = payload,
            SuffixSalt = suffixSalt,
            CreatedAt = now,
            ExpiresAt = expiresAt,
        };
    }

    public virtual string CreateEncryptedToken(Identity identity)
    {
        return TokenEncrypter.Encrypt(CreateToken(identity));
    }

    /// <summary>
    /// Extracts and decrypts the bearer token from the provided HTTP context.
    /// </summary>
    protected virtual OperationResult<Token> TryGetTokenFromContext(HttpContext httpContext)
    {
        var rawToken = httpContext.GetBearerToken();

        if (rawToken == null)
        {
            return new(true);
        }

        if (!TokenEncrypter.Verify(rawToken))
        {
            var message = "The provided bearer token is not recognized by the encryption algorithm. Please verify that the token's value adheres to the expected format and encryption standards used by the system.";
            var error = new Error(message);

            return new(true, null, error);
        }

        return new(TokenEncrypter.Decrypt(rawToken));
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
        var keyStorage = new EncryptionKeyStorage("websession_key", 32);
        keyStorage.MissingFileStrategy = EncryptionKeyStorage.OnMissingFile.CreateNew;
        return keyStorage.GetBytesAsync().Result;
    }

    byte[] InitializationVector()
    {
        var keyStorage = new EncryptionKeyStorage("websession_iv", 16);
        keyStorage.MissingFileStrategy = EncryptionKeyStorage.OnMissingFile.CreateNew;
        return keyStorage.GetBytesAsync().Result;
    }

    /// <summary>
    /// Returns the token encrypter, which is initialized with the encryption key.
    /// </summary>
    ITokenEncrypter CreateTokenEncrypter()
    {
        return new TokenEncrypter(new AesEncrypter(EncryptionKey(), InitializationVector()));
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

}
