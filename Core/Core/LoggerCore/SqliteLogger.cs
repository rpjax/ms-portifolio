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
    /// Gets or sets the flags of the log entry.
    /// </summary>
    public string? Flags { get; set; } = null;
}

public class SqliteLogReader<T> : ILogReader<T> where T : EFLogEntry
{
    protected EFCoreSqliteContext<T> Context { get; init; }

    public SqliteLogReader(FileInfo file)
    {
        Context = new EFCoreSqliteContext<T>(file);
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    public IQueryable<T> AsQueryable()
    {
        return Context.Entries.AsQueryable();
    }

    public IAsyncEnumerable<T> AsAsyncQueryable()
    {
        return Context.Entries.AsAsyncEnumerable();
    }
    
}

public class SqliteLogWriter<T> : ILogWriter<T> where T : EFLogEntry
{
    EFCoreSqliteContext<T> context;

    public SqliteLogWriter(FileInfo file)
    {
        context = new EFCoreSqliteContext<T>(file);
    }

    public void Dispose()
    {
        context.Dispose();
    }

    public async Task WriteAsync(T entry)
    {
        await context.Entries.AddAsync(entry);
        await context.SaveChangesAsync();
    }

    public async Task WriteAsync(IEnumerable<T> entries)
    {
        await context.Entries.AddRangeAsync(entries);
        await context.SaveChangesAsync();
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