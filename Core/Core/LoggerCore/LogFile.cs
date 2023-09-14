namespace ModularSystem.Core.Logging;

//*
// The new log API starts here.
//*

//*
// Notes:
// The log file interpreter is based on the lexer and will read using the enumerator of lexer tokens.
// The interpreter uses the syntax grammar of the log format to find sections and form entries.
//
// To read an entry the interpreter advaces the tokens stream until the entry delimiter(';') is found, then the key-value pairs
// are parsed from the gathered tokens and the interpreter yield returns a LogEntry.
//
// Further more that will be used to implement IQueryable in the future.
//*


/// <summary>
/// A wrapper for key-value pairs. It can be interpreted as a header or an entry property.
/// </summary>
public partial class Pairs
{
    Dictionary<string, string> pairs;

    public Pairs(Dictionary<string, string>? pairs = null)
    {
        this.pairs = pairs ?? new();
    }

    public static string FormatKey(string key)
    {
        return key.ToLower().Replace("_", "");
    }

    public virtual Dictionary<string, string> ToDictionary()
    {
        return pairs;
    }

    public void Add(string key, string value)
    {
        if (Contains(key))
        {
            throw new ArgumentException($"Duplicated key: {key}");
        }

        pairs[FormatKey(key)] = value;
    }

    public void AddNullable(string key, object? value)
    {
        if (value == null)
        {
            return;
        }

        if (Contains(key))
        {
            throw new ArgumentException($"Duplicated key: {key}");
        }

        string? str = value.ToString();

        if (str == null)
        {
            return;
        }

        pairs[FormatKey(key)] = str;
    }

    public void Remove(string key)
    {
        var pair = FindNullable(key);

        if (pair != null)
        {
            pairs.Remove(pair.Value.Key);
        }
    }

    public void Clear()
    {
        pairs.Clear();
    }

    public bool Contains(string key)
    {
        return pairs.Keys.Where(x => FormatKey(x) == FormatKey(key)).IsNotEmpty();
    }

    public KeyValuePair<string, string> Find(string key)
    {
        string formatedKey = FormatKey(key);
        var query = pairs.Where(x => FormatKey(x.Key) == FormatKey(formatedKey));

        if (query.IsEmpty())
        {
            throw new ArgumentException();
        }

        return query.First();
    }

    public KeyValuePair<string, string>? FindNullable(string key)
    {
        string formatedKey = FormatKey(key);
        var query = pairs.Where(x => FormatKey(x.Key) == FormatKey(formatedKey));

        if (query.IsEmpty())
        {
            return null;
        }

        return query.First();
    }

    public string Get(string key)
    {
        return Find(key).Value;
    }

    public string? GetNullable(string key)
    {
        return FindNullable(key)?.Value;
    }

    public int GetInt(string key)
    {
        return int.Parse(Find(key).Value);
    }

    public int? GetNullableInt(string key)
    {
        var pair = FindNullable(key);

        if (pair == null)
        {
            return null;
        }

        return int.Parse(pair.Value.Value);
    }

    public long GetLong(string key)
    {
        return long.Parse(Find(key).Value);
    }

    public long? GetNullableLong(string key)
    {
        var pair = FindNullable(key);

        if (pair == null)
        {
            return null;
        }

        return long.Parse(pair.Value.Value);
    }

    public DateTime GetDateTime(string key)
    {
        return DateTime.Parse(Find(key).Value);
    }

    public DateTime? GetNullableDateTime(string key)
    {
        var pair = FindNullable(key);

        if (pair == null)
        {
            return null;
        }

        return DateTime.Parse(pair.Value.Value);
    }
}

public class LogSection
{
    public string Name { get; set; }
    public Pairs Pairs { get; set; }

    public LogSection(string name, Pairs? pairs = null)
    {
        Name = name;
        Pairs = pairs ?? new();
    }
}

public class HeaderSection
{
    public const string SECTION_NAME = "HEADER";

    public long? InsertionCounter { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastInsertionAt { get; set; }
    public DateTime? LastEntryWipeAt { get; set; }

    public HeaderSection()
    {
        Description ??= "This log has not been properly initialized and should not be used.";
        InsertionCounter ??= 0;
        CreatedAt ??= TimeProvider.Now();
    }

    public HeaderSection(Pairs pairs)
    {
        InsertionCounter = pairs.GetNullableLong(nameof(InsertionCounter));
        Description = pairs.GetNullable(nameof(Description));
        CreatedAt = pairs.GetNullableDateTime(nameof(CreatedAt));
        LastInsertionAt = pairs.GetNullableDateTime(nameof(LastInsertionAt));
        LastEntryWipeAt = pairs.GetNullableDateTime(nameof(LastEntryWipeAt));
    }

    public HeaderSection(Dictionary<string, string> pairs) : this(new Pairs(pairs)) { }

    public Pairs Serialize()
    {
        var pairs = new Pairs();
        Serialize(pairs);
        return pairs;
    }

    protected virtual void Serialize(Pairs pairs)
    {
        pairs.AddNullable(nameof(InsertionCounter), StringifyLong(InsertionCounter));
        pairs.AddNullable(nameof(Description), Description);
        pairs.AddNullable(nameof(CreatedAt), StringifyDateTime(CreatedAt));
        pairs.AddNullable(nameof(LastInsertionAt), StringifyDateTime(LastInsertionAt));
        pairs.AddNullable(nameof(LastEntryWipeAt), StringifyDateTime(LastEntryWipeAt));
    }

    /// <summary>
    /// Returns a string of length 19.
    /// </summary>
    /// <returns></returns>
    public static string StringifyLong(long? number)
    {
        string str = number.ToString() ?? "0";
        return str.PadLeft(19, '0');
    }

    public static string StringifyDateTime(DateTime? dateTime)
    {
        return dateTime?.ToString() ?? DateTime.MinValue.ToString();
    }
}

public class EntriesSection
{
    public const string SECTION_NAME = "ENTRIES";
}

/// <summary>
/// This class will be totally reworked in the future.
/// </summary>
[Serializable]
public class Entry
{
    public long? InsertionId { get; set; } = null;
    public string? Type { get; set; } = null;
    public string? Message { get; set; } = null;
    public string? StackTrace { get; set; } = null;
    public string? Hash { get; set; } = null;
    public DateTime? CreatedAt { get; set; } = null;

    public Entry()
    {
        CreatedAt ??= TimeProvider.Now();
    }

    public Entry(Pairs pairs)
    {
        InsertionId = pairs.GetNullableLong(nameof(InsertionId));
        Type = pairs.GetNullable(nameof(Type));
        Message = pairs.GetNullable(nameof(Message));
        StackTrace = pairs.GetNullable(nameof(StackTrace));
        Hash = pairs.GetNullable(nameof(Hash));
        CreatedAt = pairs.GetNullableDateTime(nameof(CreatedAt)) ?? pairs.GetNullableDateTime("time");
    }

    public Entry(Dictionary<string, string> pairs) : this(new Pairs(pairs)) { }

    public override string ToString()
    {
        return $"[{Type}]: ({CreatedAt}) \"{Message}\";";
    }

    public Pairs Serialize()
    {
        return Serialize(new Pairs());
    }

    public virtual Pairs Serialize(Pairs pairs)
    {
        pairs.AddNullable("time", CreatedAt);
        pairs.AddNullable(nameof(Type), Type);
        pairs.AddNullable(nameof(InsertionId), InsertionId);
        pairs.AddNullable(nameof(Message), Message);
        pairs.AddNullable(nameof(StackTrace), StackTrace);
        pairs.AddNullable(nameof(Hash), Hash);
        return pairs;
    }
}

public interface ILogEntry : IQueryableModel
{
    string? Type { get; set; }
    string? Message { get; set; }
    string? StackTrace { get; set; }
}

public static class LogTypes
{
    public const string Log = "LOG";
    public const string Error = "ERROR";
    public const string Info = "INFO";
    public const string Warn = "WARN";
}