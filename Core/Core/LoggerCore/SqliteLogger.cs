using ModularSystem.EntityFramework;

namespace ModularSystem.Core.Logging;

/// <summary>
/// Represents a log entry designed for Entity Framework storage.
/// </summary>
/// <remarks>
/// This class extends <see cref="EFModel"/> to provide the structure required for logging entries with Entity Framework. <br/>
/// It implements the <see cref="ILogEntry"/> to ensure compatibility with general logging mechanisms.
/// </remarks>
public class EFLogEntry : EFModel, ILogEntry
{
    /// <summary>
    /// Gets or sets the type of the log entry. This can be an error, info, warning, etc.
    /// </summary>
    public string? Type { get; set; } = null;

    /// <summary>
    /// Gets or sets the main content or details of the log entry.
    /// </summary>
    public string? Message { get; set; } = null;

    /// <summary>
    /// Gets or sets the stack trace at the point where the log was generated, if applicable.
    /// </summary>
    public string? StackTrace { get; set; } = null;
}

public class SqliteLogReader<T> : ILogReader<T> where T : EFLogEntry
{
    EFCoreContext<T> context;

    public SqliteLogReader(FileInfo file)
    {
        context = new EFCoreContext<T>(file);
    }

    public void Dispose()
    {
        context.Dispose();
    }

    public IQueryable<T> GetEntries()
    {
        return context.Entries.AsQueryable();
    }
}

public class SqliteLogWriter<T> : ILogWriter<T> where T : EFLogEntry
{
    EFCoreContext<T> context;

    public SqliteLogWriter(FileInfo file)
    {
        context = new EFCoreContext<T>(file);
    }

    public void Dispose()
    {
        context.Dispose();
    }

    public void Write(T entry)
    {
        context.Entries.Add(entry);
        context.SaveChanges();
        DeleteWalFile();
    }

    public void Write(IEnumerable<T> entries)
    {
        context.Entries.AddRange(entries);
        context.SaveChanges();
        DeleteWalFile();
    }

    void DeleteWalFile()
    {
        context.DeleteWalFile();
    }
}

public class SqliteLogger<T> : Logger<T> where T : EFLogEntry
{
    FileInfo FileInfo { get; }

    public SqliteLogger(FileInfo fileInfo)
    {
        FileInfo = fileInfo;
    }

    public override FileInfo GetFileInfo()
    {
        return FileInfo;
    }

    public override ILogReader<T> GetReader()
    {
        return new SqliteLogReader<T>(FileInfo);
    }

    public override ILogWriter<T> GetWriter()
    {
        return new SqliteLogWriter<T>(FileInfo);
    }
}