namespace ModularSystem.Web.Authentication;


//*
// WORK IN PROGRESS, SESSION BASED AUTH.
//*

///// <summary>
///// Default implementation of <see cref="IAuthenticationProvider"/> that uses SQLite to store it's data as <see cref="EFSession"/>.
///// </summary>
//public class EFSessionAuthenticationProvider : IAuthenticationProvider, IAsyncAuthenticationProvider
//{
//    /// <summary>
//    /// Defaults to 12 hours.
//    /// </summary>
//    public static TimeSpan DefaultTokenLifetime { get; } = TimeSpan.FromHours(12);

//    /// <summary>
//    /// Defaults to 24 hours.
//    /// </summary>
//    public static TimeSpan DefaultSessionLifetime { get; } = TimeSpan.FromHours(24);

//    /// <summary>
//    /// Defaults to 15 seconds.
//    /// </summary>
//    public static TimeSpan DefaultCleanerRoutineExecutionInterval { get; } = TimeSpan.FromSeconds(15);

//    public TimeSpan TokenLifetime { get; set; }
//    public TimeSpan SessionLifetime { get; set; }

//    static byte[]? EncryptionKeyCache { get; set; } = null;

//    ITokenEncrypter TokenEncrypter { get; }
//    ISaltGenerator SaltGenerator { get; }
//    SingleThreadRoutine CleanerRoutine { get; }

//    public EFSessionAuthenticationProvider(Parameters? parameters = null)
//    {
//        parameters ??= new Parameters();

//        TokenLifetime = parameters.TokenLifetime;
//        SessionLifetime = parameters.SessionLifetime;
//        TokenEncrypter = GetTokenEncrypter();
//        SaltGenerator = new SaltGenerator();
//        CleanerRoutine = new SingleThreadRoutine(parameters.CleanerRoutineExecutionInterval, new CleanerSubRoutine());

//        CleanerRoutine.Start();
//    }

//    public static EFCoreContext<EFSession> StorageContext()
//    {
//        var fileInfo = LocalStorage.GetFileInfo("WebSessions.db");
//        return new EFCoreContext<EFSession>(fileInfo);
//    }

//    public static Entity<EFSession> GetEntity()
//    {
//        return new SessionEntity();
//    }

//    public ITokenEncrypter GetTokenEncrypter()
//    {
//        var encrypter = new AesEncrypter(EncryptionKey());
//        return new TokenEncrypter(encrypter);
//    }

//    public IToken? GetToken(HttpContext httpContext)
//    {
//        var rawToken = httpContext.GetBearerToken();

//        if (rawToken == null || !TokenEncrypter.Verify(rawToken))
//        {
//            return null;
//        }

//        return TokenEncrypter.Decrypt(rawToken);
//    }

//    public IIdentity? GetIdentity(IToken token)
//    {
//        return GetIdentityAsync(token).Result;
//    }

//    public Task<IToken?> GetTokenAsync(HttpContext httpContext)
//    {
//        return Task.FromResult(GetToken(httpContext));
//    }

//    public async Task<IIdentity?> GetIdentityAsync(IToken token)
//    {
//        using var entity = new SessionEntity();
//        var identityId = token.Payload;
//        var query = new Query<EFSession>(x => x.IdentityIdentifier == identityId);
//        var queryResult = await entity.QueryAsync(query);

//        if (queryResult.IsEmpty)
//        {
//            throw new AppException("The credentials have expired.", ExceptionCode.CredentialsExpired);
//        }

//        return queryResult.First.GetIdentity();
//    }

//    public IToken CreateTokenFrom(IIdentity identity)
//    {
//        var random = new Random();
//        var prefixSaltSize = random.Next(100, 500);
//        var sufixSaltSize = random.Next(100, 500);
//        var now = TimeProvider.Now();
//        var lifetime = TokenLifetime;

//        return new Token()
//        {
//            PrefixSalt = SaltGenerator.Generate(prefixSaltSize),
//            Payload = identity.UniqueIdentifier,
//            SuffixSalt = SaltGenerator.Generate(sufixSaltSize),
//            CreatedAt = now,
//            ExpiresAt = now.Add(lifetime),
//        };
//    }

//    public EFSession CreateSessionFrom(IIdentity identity)
//    {
//        if (identity is not Identity)
//        {
//            throw new ArgumentException("This implementation of AuthenticationProvider only works with the default Identity.");
//        }

//        var _identity = identity.TypeCast<Identity>();
//        var now = TimeProvider.Now();
//        var lifetime = SessionLifetime;

//        var session = new EFSession()
//        {
//            IdentityIdentifier = identity.UniqueIdentifier,
//            CreatedAt = now,
//            ExpiresAt = now.Add(lifetime),
//        };

//        return session.SetIdentity(_identity);
//    }

//    static void InitEncryptionKeyCache()
//    {
//        var storage = new JsonStorage<KeyFile>("websession_encryption_key");
//        var file = storage.Read();

//        //*
//        // Initializes a new random key.
//        //*
//        if (file.Bytes == null || file.Bytes.IsEmpty())
//        {
//            file.Bytes = AesEncrypter.RandomKey(AesKeySize.bits256);
//            storage.Write(file);
//        }

//        EncryptionKeyCache = file.Bytes;
//    }

//    static byte[] EncryptionKey()
//    {
//        if (EncryptionKeyCache == null)
//        {
//            InitEncryptionKeyCache();
//        }

//        return EncryptionKeyCache!;
//    }

//    public class Parameters
//    {
//        public TimeSpan TokenLifetime { get; set; } = DefaultTokenLifetime;

//        public TimeSpan SessionLifetime { get; set; } = DefaultSessionLifetime;

//        public TimeSpan CleanerRoutineExecutionInterval { get; set; } = DefaultCleanerRoutineExecutionInterval;
//    }

//    public class SessionEntity : EFEntity<EFSession>
//    {
//        public override IDAO<EFSession> DataAccessObject { get; }
//        public override IValidator<EFSession> Validator { get; }
//        public override IValidator<EFSession> UpdateValidator { get; }

//        public SessionEntity()
//        {
//            DataAccessObject = new EFCoreDAO<EFSession>(StorageContext());
//            Validator = new SessionValidator();
//            UpdateValidator = new EmptyValidator<EFSession>();
//        }
//    }

//    internal class SessionValidator : IValidator<EFSession>
//    {
//        public async Task ValidateAsync(EFSession instance)
//        {
//            using var entity = new SessionEntity();
//            var identifier = instance.GetIdentity().UniqueIdentifier;
//            var count = await entity.CountAsync(x => x.IdentityIdentifier != null && x.IdentityIdentifier == identifier);

//            if (count > 0)
//            {
//                throw new Exception("This identity already has a session.");
//            }
//        }
//    }

//    internal class KeyFile
//    {
//        public byte[]? Bytes { get; set; } = new byte[0];
//    }

//    internal class CleanerSubRoutine : SubRoutine
//    {
//        public override void Execute()
//        {
//            using var entity = new SessionEntity();
//            var now = TimeProvider.Now();
//            var ids = entity.AsQueryable()
//                .Where(x => x.ExpiresAt < now)
//                .Select(x => x.Id)
//                .ToList()
//                .ConvertAll(x => x.ToString())
//                .ToArray();

//            entity.DeleteAsync(ids).Wait();
//        }
//    }
//}
