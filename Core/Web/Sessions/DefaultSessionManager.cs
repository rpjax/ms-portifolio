using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Core.Cryptography;
using ModularSystem.Core.Helpers;
using ModularSystem.Core.Security;
using ModularSystem.Core.Threading;
using ModularSystem.EntityFramework;
using ModularSystem.Web.Authentication;

namespace ModularSystem.Web;

/// <summary>
/// Default implementation of <see cref="ISessionManager"/> that uses SQLite to store it's data.
/// </summary>
public class DefaultSessionManager : EFEntity<EFSession>, ISessionManager
{
    public static string DefaultEncodedSessionQueryParamName { get; set; } = "";
    /// <summary>
    /// Defaults to 15 seconds.
    /// </summary>
    public static TimeSpan DefaultCleanerRoutineExecutionInterval { get; set; } = TimeSpan.FromSeconds(15);

    static byte[]? DefaultEncryptionKeyCache { get; set; } = null;

    public bool EnableHttpForwarding { get; set; }
    public override IDataAccessObject<EFSession> DataAccessObject { get => new EFCoreDataAccessObject<EFSession>(DefaultStorageContext()); }

    ITokenEncrypter TokenEncrypter { get; }
    SingleThreadRoutine CleanerRoutine { get; }

    public DefaultSessionManager()
    {
        UpdateValidator = new DefaultSessionValidator();
        CleanerRoutine = new SingleThreadRoutine(DefaultCleanerRoutineExecutionInterval, new CleanerSubRoutine());

        CleanerRoutine.Start();
    }

    public static DefaultSessionManager CreateInstance()
    {
        return new DefaultSessionManager();
    }

    public static EFCoreContext<EFSession> DefaultStorageContext()
    {
        var fileInfo = LocalStorage.GetFileInfo("WebSessions.db");
        return new EFCoreContext<EFSession>(fileInfo);
    }

    public override void Dispose()
    {
        CleanerRoutine.Dispose();
        base.Dispose();
    }

    public async Task<ISession?> GetSessionAsync(HttpContext context)
    {
        var rawToken = context.GetBearerToken();

        if (rawToken == null || !TokenEncrypter.Verify(rawToken))
        {
            return null;
        }

        var token = TokenEncrypter.Decrypt(rawToken);

        using var entity = new DefaultSessionEntity();

        if (!long.TryParse(token.Payload, out var id))
        {
            throw new AppException("Invalid credentials provided. The bearer token is not valid.", ExceptionCode.CredentialsInvalid);
        }

        var query = new Query<EFSession>(x => x.Id == id);
        var queryResult = await entity.QueryAsync(query);

        if (queryResult.IsEmpty)
        {
            throw new AppException("The credentials have expired.", ExceptionCode.CredentialsExpired);
        }

        return queryResult.First;
    }

    public IEncrypter GetDataEncrypter(byte[] key)
    {
        return new AesEncrypter(key);
    }

    public void Encode(IIdentity session, HttpRequest request)
    {
        throw new NotImplementedException();
    }

    public IIdentity? Decode(HttpResponse response)
    {
        throw new NotImplementedException();
    }

    internal class KeyFile
    {
        public byte[]? Bytes { get; set; } = new byte[0];
    }

    public class DefaultSessionEntity : EFEntity<EFSession>
    {
        public override IDataAccessObject<EFSession> DataAccessObject { get; }

        public DefaultSessionEntity()
        {
            DataAccessObject = new EFCoreDataAccessObject<EFSession>(DefaultStorageContext());
        }
    }

    internal class DefaultSessionValidator : IValidator<EFSession>
    {
        public async Task<Exception?> ValidateAsync(EFSession instance)
        {
            using var entity = new DefaultSessionEntity();
            var identifier = instance.GetIdentity().UniqueIdentifier;
            var count = await entity.CountAsync(x => x.IdentityIdentifier != null && x.IdentityIdentifier == identifier);

            if (count > 0)
            {
                return new Exception("This identity already has a session.");
            }

            return null;
        }
    }

    internal class CleanerSubRoutine : ScheduledCallback
    {
        public override void Execute()
        {
            using var entity = new DefaultSessionEntity();
            var now = TimeProvider.Now();
            var ids = entity.AsQueryable()
                .Where(x => x.ExpiresAt < now)
                .Select(x => x.Id)
                .ToList()
                .ConvertAll(x => x.ToString())
                .ToArray();

            entity.DeleteAsync(ids).Wait();
        }
    }
}