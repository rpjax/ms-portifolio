using ModularSystem.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ModularSystem.Core.Patterns;

namespace ModularSystem.Core.Logging;

public class ErrorEntry : IEFCoreModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// Gets the title of the error.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets the description of the error.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets the code associated with the error.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets the list of flags associated with the error.
    /// </summary>
    public ErrorEntryFlag[] Flags { get; set; } = Array.Empty<ErrorEntryFlag>();

    /// <summary>
    /// Gets the dictionary of details associated with the error.
    /// </summary>
    public ErrorEntryDetail[] Details { get; set; } = Array.Empty<ErrorEntryDetail>();

    /// <summary>
    /// Gets the dictionary of debug data associated with the error.
    /// </summary>
    public ErrorEntryData[] Data { get; set; } = Array.Empty<ErrorEntryData>();

    public ErrorEntry()
    {
    }

    public ErrorEntry(Error error)
    {
        Title = error.Title;
        Description = error.Description;
        Code = error.Code;
        Flags = error.Flags.Select(x => new ErrorEntryFlag { Value = x }).ToArray();
        Details = error.Details.Select(x => new ErrorEntryDetail { Key = x.Key, Value = x.Value }).ToArray();
        Data = error.Data.Select(x => new ErrorEntryData { Key = x.Key, Value = x.Value }).ToArray();
    }

    public bool Equals(IEntity? other)
    {
        return other is ErrorEntry record && record.Id == Id;
    }

    public string GetId()
    {
        return Id.ToString();
    }
}

public class ErrorEntryFlag : IEFCoreModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string Value { get; set; }
}

public class ErrorEntryDetail : IEFCoreModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }
}

public class ErrorEntryData : IEFCoreModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }
}