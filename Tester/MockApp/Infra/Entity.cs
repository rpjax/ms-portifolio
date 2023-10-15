using ModularSystem.Core;
using ModularSystem.EntityFramework;
using ModularSystem.Mongo;
using System.Text.Json.Serialization;

namespace ModularSystem.Tester;

public class MongoAnt : MongoModel
{
    public CurrencyValue Value { get; set; } = new();
}

public class MongoAntService : MongoEntityService<MongoAnt>
{
    public override IDataAccessObject<MongoAnt> DataAccessObject { get; }

    public MongoAntService()
    {
        DataAccessObject = new MongoDataAccessObject<MongoAnt>(DatabaseSource.Ants);
    }
}

public class EFTestEntity : EFModel
{
    public string FirstName { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public EFEmail Email { get; set; } = EFEmail.Empty();
}

/// <summary>
/// A direct copy of <see cref="Mailing.Email"/>.
/// </summary>
public class EFEmail : EFModel
{
    public string Username { get; set; }
    public string Domain { get; set; }
    public string Extension { get; set; }

    [JsonConstructor]
    public EFEmail()
    {
        Username = string.Empty;
        Domain = string.Empty;
        Extension = string.Empty;
    }

    public EFEmail(string username, string domain, string extension)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        if (string.IsNullOrWhiteSpace(domain))
            throw new ArgumentException("Domain cannot be empty.", nameof(domain));

        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("Extension cannot be empty.", nameof(extension));

        Username = username;
        Domain = domain;
        Extension = extension;
    }

    public EFEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        var parts = email.Split('@');

        if (parts.Length != 2)
            throw new ArgumentException("Invalid email format.", nameof(email));

        Username = parts[0];

        var domainParts = parts[1].Split('.');

        if (domainParts.Length != 2)
            throw new ArgumentException("Invalid email format.", nameof(email));

        Domain = domainParts[0];
        Extension = domainParts[1];
    }

    public static EFEmail Empty()
    {
        return new();
    }

    public override string ToString()
    {
        return $"{Username}@{Domain}.{Extension}";
    }

    public string ToSafeDisplayFormat()
    {
        if (string.IsNullOrEmpty(Username) || Username.Length < 2)
        {
            return ToString();
        }

        string maskedUsername = Username[0] + new string('*', Username.Length - 2) + Username[^1];
        return $"{maskedUsername}@{Domain}.{Extension}";
    }
}

public class EFTestService : EFEntityService<EFTestEntity>
{
    public override IDataAccessObject<EFTestEntity> DataAccessObject { get; }

    public EFTestService()
    {
        DataAccessObject = new EFCoreDataAccessObject<EFTestEntity>(EFDatabaseSource.TestContext());
    }
}